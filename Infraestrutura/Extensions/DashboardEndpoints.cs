using Microsoft.AspNetCore.Mvc;
using minimal_api.Aplicacao.Services;
using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Exceptions;
using minimal_api.Dominio.Interfaces;

namespace minimal_api.Infraestrutura.Extensions;

/// <summary>
/// Extensão para mapeamento de endpoints de dashboard
/// Task 3.3: Create GET /dashboard endpoint
/// Implementa Requirements 6: Dashboard com Visualização de Saldo e Gráficos
/// </summary>
public static class DashboardEndpoints
{
    /// <summary>
    /// Mapeia todos os endpoints de dashboard
    /// </summary>
    public static void MapDashboardEndpoints(this WebApplication app)
    {
        var dashboardGroup = app.MapGroup("/api/dashboard")
            .WithTags("Dashboard");

        // GET /api/dashboard - Endpoint principal do dashboard
        // Task 3.3: Retornar saldo total, receitas totais, despesas totais
        // Requirements 6: Dashboard com Visualização de Saldo e Gráficos
        dashboardGroup.MapGet("/", async (
            IDashboardService dashboardService,
            CancellationToken cancellationToken,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int? userId = null) =>
        {
            try
            {
                // Criar filtro com defaults aplicados
                var filter = new DashboardFilterDto
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    UserId = userId
                };
                filter.ApplyDefaults();

                // Validar período
                if (!filter.IsValid())
                {
                    return Results.BadRequest(new
                    {
                        error = "Invalid date range",
                        detail = "Start date must be less than or equal to end date",
                        timestamp = DateTime.UtcNow
                    });
                }

                // Obter totais de receitas, despesas e saldo
                // Requirement 6.1: exibir saldo total em destaque no topo
                var (totalIncome, totalExpenses, balance) = await dashboardService.GetTotalsForPeriodAsync(
                    filter.StartDate,
                    filter.EndDate,
                    filter.UserId,
                    cancellationToken);

                // Construir resposta do dashboard
                var response = new DashboardResponseDto
                {
                    Balance = balance,
                    TotalIncome = totalIncome,
                    TotalExpenses = totalExpenses,
                    StartDate = filter.StartDate,
                    EndDate = filter.EndDate,
                    LastUpdated = DateTime.UtcNow,
                    TransactionCount = 0 // TODO: Implementar contagem de transações quando disponível
                };

                return Results.Ok(response);
            }
            catch (ValidationException ex)
            {
                return Results.BadRequest(new
                {
                    error = "Validation failed",
                    details = ex.Errors,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    title: "Failed to load dashboard data",
                    detail: ex.Message,
                    statusCode: 500
                );
            }
        })
        .WithName("GetDashboard")
        .WithSummary("Obter dados do dashboard")
        .WithDescription("Retorna saldo total, receitas totais, despesas totais com indicador visual se saldo negativo. Inclui gráficos de distribuição por categoria e evolução mensal.")
        .Produces<DashboardResponseDto>(200)
        .Produces<object>(400)
        .Produces<object>(500);

        // GET /api/dashboard/summary - Resumo rápido do dashboard
        // Endpoint otimizado para carregamento inicial mais rápido
        dashboardGroup.MapGet("/summary", async (
            IDashboardService dashboardService,
            CancellationToken cancellationToken,
            [FromQuery] int? userId = null) =>
        {
            try
            {
                var (totalIncome, totalExpenses, balance) = await dashboardService.GetTotalsAsync(
                    userId,
                    cancellationToken);

                var response = new DashboardResponseDto
                {
                    Balance = balance,
                    TotalIncome = totalIncome,
                    TotalExpenses = totalExpenses,
                    StartDate = DateTime.MinValue,
                    EndDate = DateTime.UtcNow,
                    TransactionCount = 0 // TODO: Implementar contagem total de transações
                };

                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    title: "Failed to load dashboard summary",
                    detail: ex.Message,
                    statusCode: 500
                );
            }
        })
        .WithName("GetDashboardSummary")
        .WithSummary("Resumo rápido do dashboard")
        .WithDescription("Retorna informações essenciais do dashboard para carregamento rápido")
        .Produces<DashboardResponseDto>(200)
        .Produces<object>(500);

        // GET /api/dashboard/balance - Endpoint específico para saldo
        // Requirements 5: Cálculo Automático de Saldo
        dashboardGroup.MapGet("/balance", async (
            IDashboardService dashboardService,
            CancellationToken cancellationToken,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int? userId = null) =>
        {
            try
            {
                decimal balance;
                
                if (startDate.HasValue || endDate.HasValue)
                {
                    // Calcular saldo para período específico
                    balance = await dashboardService.GetBalanceForPeriodAsync(
                        startDate,
                        endDate,
                        userId,
                        cancellationToken);
                }
                else
                {
                    // Calcular saldo total
                    balance = await dashboardService.GetBalanceAsync(userId, cancellationToken);
                }

                var isNegative = balance < 0;

                return Results.Ok(new
                {
                    balance = balance,
                    isNegative = isNegative,
                    period = new
                    {
                        startDate = startDate?.ToString("yyyy-MM-dd"),
                        endDate = endDate?.ToString("yyyy-MM-dd")
                    },
                    userId = userId,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (ValidationException ex)
            {
                return Results.BadRequest(new
                {
                    error = "Invalid parameters",
                    details = ex.Errors,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    title: "Failed to calculate balance",
                    detail: ex.Message,
                    statusCode: 500
                );
            }
        })
        .WithName("GetDashboardBalance")
        .WithSummary("Calcular saldo do dashboard")
        .WithDescription("Calcula o saldo total ou para período específico com indicador visual se negativo")
        .Produces<object>(200)
        .Produces<object>(400)
        .Produces<object>(500);

        // GET /api/dashboard/category-distribution - Distribuição de despesas por categoria
        // Task 3.5: Create CategoryDistribution DTO e endpoint
        // Requirements 6.2: gráfico de pizza mostrando distribuição de despesas por categoria
        // Task 3.11: Create period filter integration no dashboard (add startDate, endDate)
        dashboardGroup.MapGet("/category-distribution", async (
            IDashboardService dashboardService,
            ICategoryRepository categoryRepository,
            CancellationToken cancellationToken,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int? userId = null) =>
        {
            try
            {
                // Criar filtro com defaults aplicados
                var filter = new DashboardFilterDto
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    UserId = userId
                };
                filter.ApplyDefaults();

                // Validar período
                if (!filter.IsValid())
                {
                    return Results.BadRequest(new
                    {
                        error = "Invalid date range",
                        detail = "Start date must be less than or equal to end date",
                        timestamp = DateTime.UtcNow
                    });
                }

                // Obter distribuição de despesas por categoria
                var distribution = await dashboardService.GetExpenseDistributionByCategoryAsync(
                    filter.StartDate,
                    filter.EndDate,
                    filter.UserId,
                    cancellationToken);

                // Obter total de despesas para cálculo de percentuais
                var (_, totalExpenses, _) = await dashboardService.GetTotalsForPeriodAsync(
                    filter.StartDate,
                    filter.EndDate,
                    filter.UserId,
                    cancellationToken);

                // Obter informações de categorias para nomes, ícones e cores
                var categories = await categoryRepository.GetActiveCategoriesAsync();
                var categoryMap = categories.ToDictionary(c => c.Id);

                // Construir resposta com informações de categorias
                var categoryItems = distribution
                    .Select(kvp =>
                    {
                        var amount = kvp.Value;
                        var percentage = totalExpenses > 0
                            ? Math.Round((amount / totalExpenses) * 100, 2)
                            : 0;

                        var categoryInfo = categoryMap.ContainsKey(kvp.Key)
                            ? categoryMap[kvp.Key]
                            : null;

                        return new CategoryDistributionItemDto
                        {
                            CategoryId = kvp.Key,
                            CategoryName = categoryInfo?.Name ?? $"Category {kvp.Key}",
                            CategoryIcon = categoryInfo?.IconName ?? string.Empty,
                            CategoryColor = categoryInfo?.Color ?? "#999999",
                            Amount = amount,
                            Percentage = percentage
                        };
                    })
                    .OrderByDescending(x => x.Amount)
                    .ToList();

                var response = new CategoryDistributionResponseDto
                {
                    Categories = categoryItems,
                    TotalExpenses = totalExpenses,
                    StartDate = filter.StartDate,
                    EndDate = filter.EndDate,
                    LastUpdated = DateTime.UtcNow
                };

                return Results.Ok(response);
            }
            catch (ValidationException ex)
            {
                return Results.BadRequest(new
                {
                    error = "Validation failed",
                    details = ex.Errors,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    title: "Failed to load category distribution",
                    detail: ex.Message,
                    statusCode: 500
                );
            }
        })
        .WithName("GetCategoryDistribution")
        .WithSummary("Obter distribuição de despesas por categoria")
        .WithDescription("Retorna gráfico de pizza com distribuição de despesas por categoria no período especificado")
        .Produces<CategoryDistributionResponseDto>(200)
        .Produces<object>(400)
        .Produces<object>(500);

        // GET /api/dashboard/monthly-trend - Tendência mensal do saldo
        // Task 3.8: Create MonthlyTrend DTO e endpoint
        // Requirements 6.3: gráfico de linha mostrando evolução do saldo ao longo dos últimos 12 meses
        // Task 3.11: Create period filter integration no dashboard (add startDate, endDate for context)
        dashboardGroup.MapGet("/monthly-trend", async (
            IDashboardService dashboardService,
            CancellationToken cancellationToken,
            [FromQuery] int monthsBack = 12,
            [FromQuery] int? userId = null) =>
        {
            try
            {
                // Validar número de meses
                if (monthsBack <= 0 || monthsBack > 60)
                {
                    return Results.BadRequest(new
                    {
                        error = "Invalid months parameter",
                        detail = "months parameter must be between 1 and 60",
                        timestamp = DateTime.UtcNow
                    });
                }

                // Obter tendência mensal detalhada com receitas e despesas
                var monthlyTrendDetailed = await dashboardService.GetMonthlyTrendDetailedAsync(
                    monthsBack,
                    userId,
                    cancellationToken);

                // Construir resposta com dados de tendência formatados
                var trendItems = monthlyTrendDetailed
                    .OrderBy(kvp => kvp.Key)
                    .Select(kvp => new MonthlyTrendItemDto
                    {
                        Month = kvp.Key,
                        Balance = kvp.Value.Balance,
                        Income = kvp.Value.Income,
                        Expenses = kvp.Value.Expenses
                    })
                    .ToList();

                var response = new MonthlyTrendResponseDto
                {
                    TrendData = trendItems,
                    MonthsCount = trendItems.Count,
                    LastUpdated = DateTime.UtcNow
                };

                return Results.Ok(response);
            }
            catch (ValidationException ex)
            {
                return Results.BadRequest(new
                {
                    error = "Validation failed",
                    details = ex.Errors,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    title: "Failed to load monthly trend",
                    detail: ex.Message,
                    statusCode: 500
                );
            }
        })
        .WithName("GetMonthlyTrend")
        .WithSummary("Obter tendência mensal do saldo")
        .WithDescription("Retorna gráfico de linha com evolução do saldo ao longo dos últimos N meses")
        .Produces<MonthlyTrendResponseDto>(200)
        .Produces<object>(400)
        .Produces<object>(500);
    }
}
