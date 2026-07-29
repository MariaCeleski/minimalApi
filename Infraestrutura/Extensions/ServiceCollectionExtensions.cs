using Microsoft.Extensions.DependencyInjection;
using minimal_api.Dominio.Interfaces;
using minimal_api.Infraestrutura.Repositories;

namespace minimal_api.Infraestrutura.Extensions;

/// <summary>
/// Extension methods for configuring repository services
/// Supports dependency injection registration for the Generic Repository Pattern
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all repository services with the dependency injection container
    /// Implements the Generic Repository Pattern (Task 1.4)
    /// </summary>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // Register generic repository
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        
        // Register specialized repositories
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IGoalRepository, GoalRepository>();
        // services.AddScoped<ITransactionLimitRepository, TransactionLimitRepository>();  // TODO: Fix compilation issue
        
        return services;
    }

    /// <summary>
    /// Registers application services
    /// Task 2.2: TransactionService registration
    /// Task 3.1: DashboardService registration
    /// Task 4.1: ReportService registration for monthly and category reports
    /// Task 4.8: ExportService registration for CSV export
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register transaction service
        services.AddScoped<minimal_api.Aplicacao.Services.ITransactionService, minimal_api.Aplicacao.Services.TransactionService>();
        
        // Register dashboard service - Task 3.1: Implement DashboardService with balance calculations
        services.AddScoped<minimal_api.Aplicacao.Services.IDashboardService, minimal_api.Aplicacao.Services.DashboardService>();
        
        // Register report service - Task 4.1: Implement ReportService with monthly aggregations
        services.AddScoped<minimal_api.Dominio.Interfaces.IReportService, minimal_api.Aplicacao.Services.ReportService>();
        
        // Register export service - Task 4.8: ExportService with CSV export
        services.AddScoped<minimal_api.Aplicacao.Services.IExportService, minimal_api.Aplicacao.Services.ExportService>();
        
        return services;
    }

    /// <summary>
    /// Registers FluentValidation validators for DTOs
    /// Task 2.1: DTO validations
    /// </summary>
    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        // Register transaction validators
        services.AddScoped<FluentValidation.IValidator<minimal_api.Dominio.DTOs.CreateTransactionDto>, minimal_api.Dominio.Validators.CreateTransactionDtoValidator>();
        services.AddScoped<FluentValidation.IValidator<minimal_api.Dominio.DTOs.UpdateTransactionDto>, minimal_api.Dominio.Validators.UpdateTransactionDtoValidator>();
        services.AddScoped<FluentValidation.IValidator<minimal_api.Dominio.DTOs.TransactionFilterDto>, minimal_api.Dominio.Validators.TransactionFilterDtoValidator>();
        
        return services;
    }
}