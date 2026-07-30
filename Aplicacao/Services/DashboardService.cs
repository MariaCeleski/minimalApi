using minimal_api.Dominio.Interfaces;
using minimal_api.Dominio.Entidades;
using Microsoft.Extensions.Logging;

namespace minimal_api.Aplicacao.Services;

/// <summary>
/// Serviço para funcionalidades de dashboard com cálculos de saldo
/// Implementa Requirements 5, 6: Cálculo Automático de Saldo, Dashboard com Visualização
/// Task 3.1: Implement DashboardService with balance calculations
/// </summary>
public class DashboardService : IDashboardService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<DashboardService> _logger;

    public DashboardService(
        ITransactionRepository transactionRepository,
        ICategoryRepository categoryRepository,
        ILogger<DashboardService> logger)
    {
        _transactionRepository = transactionRepository;
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    /// <summary>
    /// Calcula e retorna o saldo total do usuário
    /// Implementa Requirements 5.1, 5.5: Saldo = Σ(receitas) - Σ(despesas) com precisão de 2 casas decimais
    /// Task 3.1: Criar método GetBalance() retornando saldo total
    /// </summary>
    /// <param name="userId">ID do usuário (opcional para versão sem autenticação)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Saldo total com precisão de 2 casas decimais</returns>
    public async Task<decimal> GetBalanceAsync(int? userId = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Calculando saldo total para usuário: {UserId}", userId?.ToString() ?? "Todos");

        try
        {
            // Requirement 5.1: Implementar Σ(receitas) - Σ(despesas)
            var totalIncome = await _transactionRepository.GetTotalByTypeAsync(
                TransactionType.Income, 
                userId, 
                cancellationToken);

            var totalExpenses = await _transactionRepository.GetTotalByTypeAsync(
                TransactionType.Expense, 
                userId, 
                cancellationToken);

            // Calcular saldo total
            var balance = totalIncome - totalExpenses;

            _logger.LogInformation("Saldo calculado com sucesso: Receitas={Income}, Despesas={Expenses}, Saldo={Balance}", 
                totalIncome, totalExpenses, balance);

            // Requirement 5.5: Precisão de 2 casas decimais
            return Math.Round(balance, 2);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao calcular saldo para usuário: {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Calcula o saldo para um período específico
    /// Suporte para dashboard com filtros de período (Requirement 3)
    /// </summary>
    /// <param name="startDate">Data de início do período</param>
    /// <param name="endDate">Data de fim do período</param>
    /// <param name="userId">ID do usuário (opcional)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Saldo do período com precisão de 2 casas decimais</returns>
    public async Task<decimal> GetBalanceForPeriodAsync(
        DateTime? startDate = null, 
        DateTime? endDate = null, 
        int? userId = null, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Calculando saldo para período: {StartDate} - {EndDate}, Usuário: {UserId}", 
            startDate?.ToString("yyyy-MM-dd"), endDate?.ToString("yyyy-MM-dd"), userId?.ToString() ?? "Todos");

        try
        {
            // Aplicar defaults se não fornecidas
            startDate ??= DateTime.Now.AddDays(-30).Date;
            endDate ??= DateTime.Now.Date.AddDays(1).AddTicks(-1);

            // Calcular receitas e despesas no período
            var totalIncome = await _transactionRepository.GetTotalByTypeInPeriodAsync(
                TransactionType.Income, 
                startDate.Value, 
                endDate.Value, 
                userId, 
                cancellationToken);

            var totalExpenses = await _transactionRepository.GetTotalByTypeInPeriodAsync(
                TransactionType.Expense, 
                startDate.Value, 
                endDate.Value, 
                userId, 
                cancellationToken);

            var balance = totalIncome - totalExpenses;

            _logger.LogInformation("Saldo do período calculado: Receitas={Income}, Despesas={Expenses}, Saldo={Balance}", 
                totalIncome, totalExpenses, balance);

            return Math.Round(balance, 2);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao calcular saldo do período para usuário: {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Obtém totais de receitas e despesas separadamente
    /// Suporte para Requirements 6: Dashboard com visualização detalhada
    /// </summary>
    /// <param name="userId">ID do usuário (opcional)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Tuple com (TotalIncome, TotalExpenses, Balance)</returns>
    public async Task<(decimal TotalIncome, decimal TotalExpenses, decimal Balance)> GetTotalsAsync(
        int? userId = null, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Obtendo totais detalhados para usuário: {UserId}", userId?.ToString() ?? "Todos");

        try
        {
            var totalIncome = await _transactionRepository.GetTotalByTypeAsync(
                TransactionType.Income, 
                userId, 
                cancellationToken);

            var totalExpenses = await _transactionRepository.GetTotalByTypeAsync(
                TransactionType.Expense, 
                userId, 
                cancellationToken);

            var balance = totalIncome - totalExpenses;

            // Requirement 5.5: Precisão de 2 casas decimais para todos os valores
            return (
                Math.Round(totalIncome, 2),
                Math.Round(totalExpenses, 2),
                Math.Round(balance, 2)
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter totais para usuário: {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Obtém totais para um período específico
    /// Suporte para dashboard com filtros
    /// </summary>
    /// <param name="startDate">Data de início</param>
    /// <param name="endDate">Data de fim</param>
    /// <param name="userId">ID do usuário (opcional)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Tuple com (TotalIncome, TotalExpenses, Balance) do período</returns>
    public async Task<(decimal TotalIncome, decimal TotalExpenses, decimal Balance)> GetTotalsForPeriodAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        int? userId = null, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Obtendo totais do período: {StartDate} - {EndDate}, Usuário: {UserId}", 
            startDate?.ToString("yyyy-MM-dd"), endDate?.ToString("yyyy-MM-dd"), userId?.ToString() ?? "Todos");

        try
        {
            // Aplicar defaults se não fornecidas
            startDate ??= DateTime.Now.AddDays(-30).Date;
            endDate ??= DateTime.Now.Date.AddDays(1).AddTicks(-1);

            var totalIncome = await _transactionRepository.GetTotalByTypeInPeriodAsync(
                TransactionType.Income, 
                startDate.Value, 
                endDate.Value, 
                userId, 
                cancellationToken);

            var totalExpenses = await _transactionRepository.GetTotalByTypeInPeriodAsync(
                TransactionType.Expense, 
                startDate.Value, 
                endDate.Value, 
                userId, 
                cancellationToken);

            var balance = totalIncome - totalExpenses;

            return (
                Math.Round(totalIncome, 2),
                Math.Round(totalExpenses, 2),
                Math.Round(balance, 2)
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter totais do período para usuário: {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Verifica se o saldo atual é negativo
    /// Implementa Requirements 5.6: marcar saldo devedor visualmente
    /// </summary>
    /// <param name="userId">ID do usuário (opcional)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>True se o saldo é negativo (devedor)</returns>
    public async Task<bool> IsBalanceNegativeAsync(int? userId = null, CancellationToken cancellationToken = default)
    {
        var balance = await GetBalanceAsync(userId, cancellationToken);
        return balance < 0;
    }

    /// <summary>
    /// Obtém distribuição de despesas por categoria
    /// Suporte para Requirement 6: gráfico de pizza no dashboard
    /// </summary>
    /// <param name="startDate">Data de início (opcional)</param>
    /// <param name="endDate">Data de fim (opcional)</param>
    /// <param name="userId">ID do usuário (opcional)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Dicionário com CategoryId -> Valor gasto</returns>
    public async Task<Dictionary<int, decimal>> GetExpenseDistributionByCategoryAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        int? userId = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Obtendo distribuição de despesas por categoria: {StartDate} - {EndDate}", 
            startDate?.ToString("yyyy-MM-dd"), endDate?.ToString("yyyy-MM-dd"));

        try
        {
            var distribution = await _transactionRepository.GetCategoryDistributionAsync(
                TransactionType.Expense,
                startDate,
                endDate,
                userId,
                cancellationToken);

            // Aplicar precisão de 2 casas decimais
            return distribution.ToDictionary(
                kvp => kvp.Key,
                kvp => Math.Round(kvp.Value, 2)
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter distribuição de despesas por categoria");
            throw;
        }
    }

    /// <summary>
    /// Obtém evolução mensal do saldo
    /// Suporte para Requirement 6: gráfico de linha no dashboard
    /// </summary>
    /// <param name="monthsBack">Número de meses para retornar (padrão: 12)</param>
    /// <param name="userId">ID do usuário (opcional)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Dicionário com Mês -> Saldo acumulado</returns>
    public async Task<Dictionary<DateTime, decimal>> GetMonthlyBalanceTrendAsync(
        int monthsBack = 12,
        int? userId = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Obtendo tendência mensal do saldo: {MonthsBack} meses, Usuário: {UserId}", 
            monthsBack, userId?.ToString() ?? "Todos");

        try
        {
            var monthlyTrend = await _transactionRepository.GetMonthlyTrendAsync(
                monthsBack,
                userId,
                cancellationToken);

            // Aplicar precisão de 2 casas decimais
            return monthlyTrend.ToDictionary(
                kvp => kvp.Key,
                kvp => Math.Round(kvp.Value, 2)
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter tendência mensal do saldo");
            throw;
        }
    }

    /// <summary>
    /// Obtém evolução mensal detalhada com receitas e despesas separadas
    /// Implementa Requirement 6: gráfico de linha com evolução do saldo ao longo dos últimos 12 meses
    /// Task 3.8: Create MonthlyTrend DTO e endpoint
    /// </summary>
    /// <param name="monthsBack">Número de meses para retornar (padrão: 12)</param>
    /// <param name="userId">ID do usuário (opcional)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Dicionário com Mês -> (Receitas, Despesas, Saldo)</returns>
    public async Task<Dictionary<DateTime, (decimal Income, decimal Expenses, decimal Balance)>> GetMonthlyTrendDetailedAsync(
        int monthsBack = 12,
        int? userId = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Obtendo tendência mensal detalhada do saldo: {MonthsBack} meses, Usuário: {UserId}", 
            monthsBack, userId?.ToString() ?? "Todos");

        try
        {
            var monthlyTrendDetailed = await _transactionRepository.GetMonthlyTrendDetailedAsync(
                monthsBack,
                userId,
                cancellationToken);

            // Aplicar precisão de 2 casas decimais em todos os valores
            return monthlyTrendDetailed.ToDictionary(
                kvp => kvp.Key,
                kvp => (
                    Math.Round(kvp.Value.Income, 2),
                    Math.Round(kvp.Value.Expenses, 2),
                    Math.Round(kvp.Value.Balance, 2)
                )
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter tendência mensal detalhada do saldo");
            throw;
        }
    }
}

/// <summary>
/// Interface para o DashboardService
/// Define contrato público do serviço de dashboard
/// </summary>
public interface IDashboardService
{
    /// <summary>
    /// Calcula e retorna o saldo total do usuário
    /// Task 3.1: Método GetBalance() retornando saldo total
    /// </summary>
    Task<decimal> GetBalanceAsync(int? userId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Calcula o saldo para um período específico
    /// </summary>
    Task<decimal> GetBalanceForPeriodAsync(DateTime? startDate = null, DateTime? endDate = null, int? userId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém totais de receitas, despesas e saldo
    /// </summary>
    Task<(decimal TotalIncome, decimal TotalExpenses, decimal Balance)> GetTotalsAsync(int? userId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém totais para um período específico
    /// </summary>
    Task<(decimal TotalIncome, decimal TotalExpenses, decimal Balance)> GetTotalsForPeriodAsync(DateTime? startDate = null, DateTime? endDate = null, int? userId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica se o saldo é negativo
    /// </summary>
    Task<bool> IsBalanceNegativeAsync(int? userId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém distribuição de despesas por categoria
    /// </summary>
    Task<Dictionary<int, decimal>> GetExpenseDistributionByCategoryAsync(DateTime? startDate = null, DateTime? endDate = null, int? userId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém evolução mensal do saldo
    /// </summary>
    Task<Dictionary<DateTime, decimal>> GetMonthlyBalanceTrendAsync(int monthsBack = 12, int? userId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém evolução mensal detalhada com receitas e despesas separadas
    /// Task 3.8: Create MonthlyTrend DTO e endpoint
    /// </summary>
    Task<Dictionary<DateTime, (decimal Income, decimal Expenses, decimal Balance)>> GetMonthlyTrendDetailedAsync(
        int monthsBack = 12,
        int? userId = null,
        CancellationToken cancellationToken = default);
}