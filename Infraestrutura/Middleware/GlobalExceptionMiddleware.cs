using System.Net;
using System.Text.Json;
using minimal_api.Dominio.Exceptions;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

namespace minimal_api.Infraestrutura.Middleware;

/// <summary>
/// Global exception handling middleware for centralized error processing
/// Implements Requirements 1, 8 - Exception handling and error responses
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new ErrorResponse
        {
            TraceId = System.Diagnostics.Activity.Current?.Id ?? context.TraceIdentifier,
            Timestamp = DateTime.UtcNow
        };

        // Handle different exception types
        switch (exception)
        {
            case minimal_api.Dominio.Exceptions.ValidationException validationEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Title = "Validation Error";
                errorResponse.Message = "Um ou mais campos contêm dados inválidos";
                errorResponse.Errors = validationEx.Errors;
                _logger.LogWarning(validationEx, "Validation error occurred: {Errors}", 
                    string.Join(", ", validationEx.Errors.SelectMany(e => e.Value)));
                break;

            case FluentValidation.ValidationException fluentValidationEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Title = "Validation Error";
                errorResponse.Message = "Um ou mais campos contêm dados inválidos";
                errorResponse.Errors = fluentValidationEx.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                _logger.LogWarning(fluentValidationEx, "FluentValidation error occurred");
                break;

            case NotFoundException notFoundEx:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse.Title = "Resource Not Found";
                errorResponse.Message = notFoundEx.Message;
                errorResponse.Details = new Dictionary<string, object>
                {
                    ["ResourceType"] = notFoundEx.ResourceType,
                    ["ResourceId"] = notFoundEx.ResourceId
                };
                _logger.LogWarning(notFoundEx, "Resource not found: {ResourceType} with ID {ResourceId}", 
                    notFoundEx.ResourceType, notFoundEx.ResourceId);
                break;

            case BusinessRuleException businessEx:
                response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                errorResponse.Title = "Business Rule Violation";
                errorResponse.Message = businessEx.Message;
                errorResponse.Details = new Dictionary<string, object>
                {
                    ["BusinessRule"] = businessEx.BusinessRule,
                    ["Context"] = businessEx.Context
                };
                _logger.LogWarning(businessEx, "Business rule violation: {BusinessRule} in context {Context}", 
                    businessEx.BusinessRule, businessEx.Context);
                break;

            case UnauthorizedException unauthorizedEx:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse.Title = "Unauthorized";
                errorResponse.Message = unauthorizedEx.Message;
                errorResponse.Details = new Dictionary<string, object>
                {
                    ["Action"] = unauthorizedEx.Action,
                    ["Resource"] = unauthorizedEx.Resource
                };
                _logger.LogWarning(unauthorizedEx, "Unauthorized access attempt: {Action} on {Resource}", 
                    unauthorizedEx.Action, unauthorizedEx.Resource);
                break;

            case DbUpdateException dbUpdateEx:
                response.StatusCode = (int)HttpStatusCode.Conflict;
                errorResponse.Title = "Database Conflict";
                errorResponse.Message = "Erro ao salvar dados. Verifique se os dados não conflitam com registros existentes.";
                _logger.LogError(dbUpdateEx, "Database update error occurred");
                
                // In development, include more details
                if (_environment.IsDevelopment())
                {
                    errorResponse.Details = new Dictionary<string, object>
                    {
                        ["InnerException"] = dbUpdateEx.InnerException?.Message ?? "No inner exception"
                    };
                }
                break;

            case OperationCanceledException:
                response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                errorResponse.Title = "Request Timeout";
                errorResponse.Message = "A operação foi cancelada por timeout";
                _logger.LogWarning(exception, "Operation was cancelled due to timeout");
                break;

            case ArgumentException argEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Title = "Invalid Argument";
                errorResponse.Message = argEx.Message;
                _logger.LogWarning(argEx, "Invalid argument provided");
                break;

            case InvalidOperationException invalidOpEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Title = "Invalid Operation";
                errorResponse.Message = invalidOpEx.Message;
                _logger.LogWarning(invalidOpEx, "Invalid operation attempted");
                break;

            case NotSupportedException notSupportedEx:
                response.StatusCode = (int)HttpStatusCode.NotImplemented;
                errorResponse.Title = "Operation Not Supported";
                errorResponse.Message = notSupportedEx.Message;
                _logger.LogWarning(notSupportedEx, "Unsupported operation attempted");
                break;

            default:
                // Generic server error for unhandled exceptions
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.Title = "Internal Server Error";
                errorResponse.Message = _environment.IsDevelopment() 
                    ? exception.Message 
                    : "Ocorreu um erro interno no servidor. Tente novamente mais tarde.";
                
                _logger.LogError(exception, "Unhandled exception occurred");

                // In development, include stack trace
                if (_environment.IsDevelopment())
                {
                    errorResponse.Details = new Dictionary<string, object>
                    {
                        ["StackTrace"] = exception.StackTrace ?? "No stack trace available",
                        ["InnerException"] = exception.InnerException?.Message ?? "No inner exception"
                    };
                }
                break;
        }

        var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = _environment.IsDevelopment()
        });

        await response.WriteAsync(jsonResponse);
    }
}

/// <summary>
/// Standardized error response format for the API
/// </summary>
public class ErrorResponse
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string TraceId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public IDictionary<string, string[]>? Errors { get; set; }
    public IDictionary<string, object>? Details { get; set; }
}

/// <summary>
/// Extension method to register the global exception middleware
/// </summary>
public static class GlobalExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<GlobalExceptionMiddleware>();
    }
}