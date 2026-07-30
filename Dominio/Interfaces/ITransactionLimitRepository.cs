using minimal_api.Dominio.Entidades;

namespace minimal_api.Dominio.Interfaces;

/// <summary>
/// Specialized Transaction Limit Repository interface
/// Supports spending limit management and notifications (Requirement 19)
/// </summary>
public interface ITransactionLimitRepository : IRepository<TransactionLimit>
{
    // User-specific limits
    Task<IEnumerable<TransactionLimit>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    // Category-specific limits
    Task<IEnumerable<TransactionLimit>> GetByCategoryIdAsync(int categoryId, CancellationToken cancellationToken = default);

    // Find specific limit
    Task<TransactionLimit?> FindByCategoryAndUserAsync(int categoryId, int? userId = null, CancellationToken cancellationToken = default);

    // Update current spending
    Task UpdateCurrentSpentAsync(int limitId, decimal newAmount, CancellationToken cancellationToken = default);

    // Calculate current spending for a limit
    Task<decimal> CalculateCurrentSpentAsync(int categoryId, LimitPeriod period, int? userId = null, CancellationToken cancellationToken = default);

    // Check if limit is exceeded
    Task<(bool isWarning, bool isExceeded)> CheckLimitStatusAsync(int limitId, CancellationToken cancellationToken = default);

    // Get all limits that need checking after a transaction
    Task<IEnumerable<TransactionLimit>> GetLimitsForTransactionAsync(int categoryId, int? userId = null, CancellationToken cancellationToken = default);

    // Limit validation
    Task<bool> ValidateLimitAsync(TransactionLimit limit, CancellationToken cancellationToken = default);

    // Reset limits for new period (monthly, weekly, etc.)
    Task ResetLimitsForPeriodAsync(LimitPeriod period, CancellationToken cancellationToken = default);
}