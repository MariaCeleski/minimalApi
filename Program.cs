using Microsoft.EntityFrameworkCore;
using minimal_api.Infraestrutura.Db;
using Serilog;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .Build())
    .CreateLogger();

try
{
    Log.Information("Starting Personal Financial Management API");

    var builder = WebApplication.CreateBuilder(args);

    // Add Serilog
    builder.Host.UseSerilog();

    // Add services to the container
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new() { 
            Title = builder.Configuration["ApiSettings:Title"] ?? "Personal Financial Management API", 
            Version = builder.Configuration["ApiSettings:Version"] ?? "v1",
            Description = builder.Configuration["ApiSettings:Description"] ?? "API para gestão financeira pessoal com controle de receitas e despesas"
        });
    });

    // Add CORS with configuration
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowReactApp", policy =>
        {
            var allowedOrigins = builder.Configuration.GetSection("CORS:AllowedOrigins").Get<string[]>() 
                ?? new[] { "http://localhost:3000", "http://localhost:5173" };
            
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
    });

    // Add Entity Framework with error handling
    builder.Services.AddDbContext<DbContexto>(options =>
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }
        options.UseSqlite(connectionString);
        
        // Enable sensitive data logging in development
        if (builder.Environment.IsDevelopment())
        {
            options.EnableSensitiveDataLogging();
        }
    });

    // Add FluentValidation
    builder.Services.AddValidatorsFromAssemblyContaining<Program>();

    // Add JWT Authentication (optional - for future use)
    var jwtSettings = builder.Configuration.GetSection("JWT");
    if (!string.IsNullOrEmpty(jwtSettings["SecretKey"]))
    {
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!))
            };
        });
        
        builder.Services.AddAuthorization();
    }

    // Add HTTP client for external services
    builder.Services.AddHttpClient();

    // Add custom services (placeholders - will be implemented in future tasks)
    // TODO: Task 2 - Add transaction services
    // builder.Services.AddScoped<ITransactionService, TransactionService>();
    // builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
    
    // TODO: Task 3+ - Add dashboard and report services  
    // builder.Services.AddScoped<IDashboardService, DashboardService>();
    // builder.Services.AddScoped<IReportService, ReportService>();
    // builder.Services.AddScoped<IExportService, ExportService>();

    var app = builder.Build();

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Personal Financial Management API v1");
            c.RoutePrefix = "swagger";
        });
    }

    // Global exception handling
    app.UseExceptionHandler(appBuilder =>
    {
        appBuilder.Run(async context =>
        {
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
            var feature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
            
            if (feature?.Error is not null)
            {
                logger.LogError(feature.Error, "Unhandled exception occurred");
            }
            
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            
            var error = new
            {
                message = app.Environment.IsDevelopment() 
                    ? feature?.Error?.Message ?? "An error occurred while processing your request."
                    : "An error occurred while processing your request.",
                timestamp = DateTime.UtcNow,
                traceId = System.Diagnostics.Activity.Current?.Id ?? context.TraceIdentifier
            };
            
            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(error));
        });
    });

    app.UseHttpsRedirection();
    
    // Request logging with Serilog
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "Handled {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
        options.GetLevel = (httpContext, elapsed, ex) => ex != null 
            ? Serilog.Events.LogEventLevel.Error 
            : httpContext.Response.StatusCode > 499 
                ? Serilog.Events.LogEventLevel.Error
                : Serilog.Events.LogEventLevel.Information;
    });

    // Use CORS
    app.UseCors("AllowReactApp");

    // Use authentication/authorization if configured
    if (!string.IsNullOrEmpty(jwtSettings["SecretKey"]))
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }

    // Health check endpoint
    app.MapGet("/health", () => Results.Ok(new { 
        status = "healthy", 
        timestamp = DateTime.UtcNow,
        version = builder.Configuration["ApiSettings:Version"] ?? "1.0.0",
        environment = app.Environment.EnvironmentName
    }))
    .WithTags("Health")
    .WithSummary("Health check endpoint")
    .WithDescription("Returns the current health status of the API")
    .Produces<object>(200);

    // API info endpoint
    app.MapGet("/", () => Results.Ok(new { 
        message = builder.Configuration["ApiSettings:Title"] ?? "Personal Financial Management API", 
        version = builder.Configuration["ApiSettings:Version"] ?? "1.0.0",
        description = builder.Configuration["ApiSettings:Description"] ?? "API para gestão financeira pessoal",
        environment = app.Environment.EnvironmentName,
        endpoints = new[] {
            "/health - Health check",
            "/swagger - API documentation"
        }
    }))
    .WithTags("Info")
    .WithSummary("API information endpoint")
    .WithDescription("Returns basic information about the API")
    .Produces<object>(200);

    // Database health check endpoint
    app.MapGet("/health/database", async (DbContexto dbContext) =>
    {
        try
        {
            await dbContext.Database.CanConnectAsync();
            return Results.Ok(new { 
                status = "healthy", 
                database = "connected",
                timestamp = DateTime.UtcNow 
            });
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "Database connection failed",
                detail: ex.Message,
                statusCode: 503
            );
        }
    })
    .WithTags("Health")
    .WithSummary("Database health check")
    .WithDescription("Verifies database connectivity")
    .Produces<object>(200)
    .Produces<object>(503);

    // TODO: Transaction endpoints will be added in Task 2
    // app.MapPost("/api/transactions", ...)
    // app.MapGet("/api/transactions", ...)
    // app.MapPut("/api/transactions/{id}", ...)
    // app.MapDelete("/api/transactions/{id}", ...)

    // TODO: Dashboard endpoints will be added in Task 3+
    // app.MapGet("/api/dashboard", ...)
    // app.MapGet("/api/reports/monthly/{year}/{month}", ...)
    // app.MapGet("/api/reports/category", ...)
    // app.MapGet("/api/export/csv", ...)
    // app.MapGet("/api/export/pdf", ...)

    // Ensure database is created and updated with migrations
    try
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DbContexto>();
        await context.Database.MigrateAsync();
        Log.Information("Database initialized successfully");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Failed to initialize database");
        throw;
    }

    Log.Information("Personal Financial Management API started successfully on {Environment}", app.Environment.EnvironmentName);
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}