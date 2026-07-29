using System.Linq.Expressions;
using minimal_api.Dominio.Entidades;

namespace minimal_api.Dominio.Interfaces;

/// <summary>
/// Specialized Transaction Repository interface with filtering capabilities
/// Supports Requirements 1, 2, 3, 4 (Transaction CRUD, Pagination, Period Filter, Category Filter)
/// </summary>
public interface ITransactionRepository : IRepository<Transaction>
{
    // Period Filtering (Requirement 3)
    Task<PagedResult<Transaction>> GetByPeriodAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    // Category Filtering (Requirement 4)
    Task<PagedResult<Transaction>> GetByCategoriesAsync(
        int[] categoryIds,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    // Combined Filtering (Requirements 3 + 4)
    Task<PagedResult<Transaction>> GetFilteredAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        int[]? categoryIds = null,
        int? userId = null,
        TransactionType? type = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    // Balance Calculation Support (Requirement 5)
    Task<decimal> CalculateBalanceAsync(
        int? userId = null,
        CancellationToken cancellationToken = default);

    Task<decimal> CalculateBalanceForPeriodAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        int? userId = null,
        CancellationToken cancellationToken = default);

    // Income/Expense Totals (Requirements 5, 6)
    Task<decimal> CalculateIncomeAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        int? userId = null,
        CancellationToken cancellationToken = default);

    Task<decimal> CalculateExpenseAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        int? userId = null,
        CancellationToken cancellationToken = default);

    // Category Distribution (Requirement 6)
    Task<Dictionary<int, decimal>> GetCategoryDistributionAsync(
        TransactionType type,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int? userId = null,
        CancellationToken cancellationToken = default);

    // Monthly Trend (Requirement 6)
    Task<Dictionary<DateTime, decimal>> GetMonthlyTrendAsync(
        int monthsBack = 12,
        int? userId = null,
        CancellationToken cancellationToken = default);

    // Transaction validation (Requirement 1)
    Task<bool> ValidateTransactionAsync(Transaction transaction, CancellationToken cancellationToken = default);

    // Additional methods for TransactionService - Task 2.2
    Task<PagedResult<Transaction>> GetPagedTransactionsAsync(
        int page,
        int pageSize,
        DateTime? startDate = null,
        DateTime? endDate = null,
        List<int>? categoryIds = null,
        TransactionType? type = null,
        int? userId = null,
        CancellationToken cancellationToken = default);

    Task<decimal> GetTotalByTypeAsync(
        TransactionType type,
        int? userId = null,
        CancellationToken cancellationToken = default);

    Task<decimal> GetTotalByTypeInPeriodAsync(
        TransactionType type,
        DateTime startDate,
        DateTime endDate,
        int? userId = null,
        CancellationToken cancellationToken = default);

    Task<int> GetCountInPeriodAsync(
        DateTime startDate,
        DateTime endDate,
        int? userId = null,
        CancellationToken cancellationToken = default);
}