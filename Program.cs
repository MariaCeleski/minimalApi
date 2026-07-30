using Microsoft.EntityFrameworkCore;
using minimal_api.Infraestrutura.Db;
using minimal_api.Infraestrutura.Middleware;
using minimal_api.Dominio.Interfaces;
using minimal_api.Infraestrutura.Repositories;
using minimal_api.Infraestrutura.Extensions;
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
    var defaultAllowedOrigins = new[] { "http://localhost:3000", "http://localhost:5173" };
    
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowReactApp", policy =>
        {
            var allowedOrigins = builder.Configuration.GetSection("CORS:AllowedOrigins").Get<string[]>() 
                ?? defaultAllowedOrigins;
            
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

    // Add repository services (Task 1.4)
    builder.Services.AddRepositories();

    // Add application services (Task 2.2)
    builder.Services.AddApplicationServices();

    // Add validators (Task 2.1)
    builder.Services.AddValidators();
    builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
    builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IGoalRepository, GoalRepository>();
    // builder.Services.AddScoped<ITransactionLimitRepository, TransactionLimitRepository>();

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

    // Global exception handling middleware - must be first
    app.UseGlobalExceptionHandler();

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
    var defaultEndpoints = new[] {
        "/health - Health check",
        "/swagger - API documentation"
    };
    
    app.MapGet("/", () => Results.Ok(new { 
        message = builder.Configuration["ApiSettings:Title"] ?? "Personal Financial Management API", 
        version = builder.Configuration["ApiSettings:Version"] ?? "1.0.0",
        description = builder.Configuration["ApiSettings:Description"] ?? "API para gestão financeira pessoal",
        environment = app.Environment.EnvironmentName,
        endpoints = defaultEndpoints
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

    // Exception testing endpoints (for development/testing only)
    if (app.Environment.IsDevelopment())
    {
        app.MapGet("/test/exceptions/validation", () =>
        {
            throw minimal_api.Dominio.Exceptions.ValidationException.ForInvalidValue(-100);
        })
        .WithTags("Test")
        .WithSummary("Test validation exception")
        .WithDescription("Throws a validation exception for testing the global exception handler")
        .ExcludeFromDescription();

        app.MapGet("/test/exceptions/notfound", () =>
        {
            throw minimal_api.Dominio.Exceptions.NotFoundException.ForTransaction(999);
        })
        .WithTags("Test")
        .WithSummary("Test not found exception")
        .WithDescription("Throws a not found exception for testing the global exception handler")
        .ExcludeFromDescription();

        app.MapGet("/test/exceptions/businessrule", () =>
        {
            throw minimal_api.Dominio.Exceptions.BusinessRuleException.ForDataIntegrityViolation("Saldo inconsistente detectado");
        })
        .WithTags("Test")
        .WithSummary("Test business rule exception")
        .WithDescription("Throws a business rule exception for testing the global exception handler")
        .ExcludeFromDescription();

        app.MapGet("/test/exceptions/unauthorized", () =>
        {
            throw minimal_api.Dominio.Exceptions.UnauthorizedException.ForInvalidCredentials();
        })
        .WithTags("Test")
        .WithSummary("Test unauthorized exception")
        .WithDescription("Throws an unauthorized exception for testing the global exception handler")
        .ExcludeFromDescription();

        app.MapGet("/test/exceptions/generic", () =>
        {
            throw new InvalidOperationException("This is a generic exception for testing");
        })
        .WithTags("Test")
        .WithSummary("Test generic exception")
        .WithDescription("Throws a generic exception for testing the global exception handler")
        .ExcludeFromDescription();
    }

    // Test endpoint for repository functionality (Task 1.4)
    if (app.Environment.IsDevelopment())
    {
        app.MapGet("/api/test/categories", async (IServiceProvider serviceProvider) =>
        {
            try
            {
                var categoryRepository = serviceProvider.GetRequiredService<ICategoryRepository>();
                var categories = await categoryRepository.GetActiveCategoriesAsync();
                return Results.Ok(new { 
                    message = "Generic Repository Pattern working correctly",
                    categoriesCount = categories.Count(),
                    categories = categories.Select(c => new { c.Id, c.Name, c.IconName, c.Color })
                });
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    title: "Repository test failed",
                    detail: ex.Message,
                    statusCode: 500
                );
            }
        })
        .WithTags("Test")
        .WithSummary("Test repository pattern")
        .WithDescription("Tests the generic repository pattern implementation")
        .Produces<object>(200)
        .Produces<object>(500);

        app.MapGet("/api/test/repository/pagination", async (IServiceProvider serviceProvider) =>
        {
            try
            {
                var categoryRepository = serviceProvider.GetRequiredService<ICategoryRepository>();
                var pagedResult = await categoryRepository.GetPagedAsync(
                    filter: null,
                    orderBy: null,
                    page: 1,
                    pageSize: 5);
                return Results.Ok(new {
                    message = "Pagination working correctly",
                    currentPage = pagedResult.CurrentPage,
                    pageSize = pagedResult.PageSize,
                    totalItems = pagedResult.TotalItems,
                    totalPages = pagedResult.TotalPages,
                    hasNextPage = pagedResult.HasNextPage,
                    hasPreviousPage = pagedResult.HasPreviousPage,
                    data = pagedResult.Data.Select(c => new { c.Id, c.Name })
                });
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    title: "Pagination test failed",
                    detail: ex.Message,
                    statusCode: 500
                );
            }
        })
        .WithTags("Test")
        .WithSummary("Test repository pagination")
        .WithDescription("Tests the repository pagination functionality")
        .Produces<object>(200)
        .Produces<object>(500);
    }

    // Transaction endpoints - Task 2.4: Create Transaction API endpoints (POST, GET, GET by ID)
    // Requirements 1, 2: Cadastro e Validação de Transações, Listagem com Paginação
    app.MapTransactionEndpoints();

    // Test endpoint to seed some sample data for pagination testing (development only)
    if (app.Environment.IsDevelopment())
    {
        app.MapPost("/api/test/seed-transactions", async (IServiceProvider serviceProvider) =>
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<DbContexto>();
                
                // Check if transactions already exist
                var existingCount = await context.Transactions.CountAsync();
                if (existingCount > 0)
                {
                    return Results.Ok(new { 
                        message = $"Database already contains {existingCount} transactions",
                        note = "Skipping seed to avoid duplicates"
                    });
                }

                // Get existing categories
                var categories = await context.Categories.Take(3).ToListAsync();
                if (!categories.Any())
                {
                    return Results.BadRequest(new { 
                        error = "No categories found. Please run database migrations first." 
                    });
                }

                // Create 25 sample transactions for pagination testing
                var transactions = new List<minimal_api.Dominio.Entidades.Transaction>();
                
                for (int i = 1; i <= 25; i++)
                {
                    transactions.Add(new minimal_api.Dominio.Entidades.Transaction
                    {
                        Amount = 50 + (i * 10),
                        Date = DateTime.Now.AddDays(-i),
                        Type = i % 3 == 0 ? minimal_api.Dominio.Entidades.TransactionType.Income : minimal_api.Dominio.Entidades.TransactionType.Expense,
                        CategoryId = categories[i % categories.Count].Id,
                        Description = $"Sample Transaction {i} for pagination testing",
                        CreatedAt = DateTime.UtcNow.AddDays(-i),
                        UpdatedAt = DateTime.UtcNow.AddDays(-i),
                        UserId = null
                    });
                }

                context.Transactions.AddRange(transactions);
                await context.SaveChangesAsync();

                return Results.Ok(new { 
                    message = $"Successfully seeded {transactions.Count} sample transactions",
                    note = "You can now test pagination with GET /api/transactions"
                });
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    title: "Failed to seed transactions",
                    detail: ex.Message,
                    statusCode: 500
                );
            }
        })
        .WithTags("Test")
        .WithSummary("Seed sample transactions for pagination testing")
        .WithDescription("Creates 25 sample transactions to test pagination functionality")
        .Produces<object>(200)
        .Produces<object>(400)
        .Produces<object>(500);
    }

    // Dashboard endpoints - Task 3.3: Create GET /dashboard endpoint
    // Requirements 6: Dashboard com Visualização de Saldo e Gráficos
    app.MapDashboardEndpoints();

    // Report endpoints - Task 4.6: Create GET /reports/category endpoint
    // Requirements 10: Relatório por Categoria
    // TODO: app.MapReportEndpoints();

    // Export endpoints - Task 4.10, 4.13: Create GET /export/csv and /export/pdf endpoints
    // Requirements 11, 12: Exportação em CSV e PDF
    app.MapExportEndpoints();

    // Limit endpoints - Task 5.8: Create CRUD endpoints para Limits
    // Requirement 19: Notificações de Limite Excedido
    app.MapLimitEndpoints();

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