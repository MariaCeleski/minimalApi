using minimal_api.Dominio.Entidades;

namespace minimal_api.Dominio.Interfaces;

/// <summary>
/// Specialized Goal Repository interface
/// Supports financial goal management (Requirement 18)
/// </summary>
public interface IGoalRepository : IRepository<Goal>
{
    // User-specific goals
    Task<IEnumerable<Goal>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<PagedResult<Goal>> GetByUserIdAsync(int userId, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);

    // Goal status filtering
    Task<IEnumerable<Goal>> GetByStatusAsync(GoalStatus status, int? userId = null, CancellationToken cancellationToken = default);

    // Active goals only
    Task<IEnumerable<Goal>> GetActiveGoalsAsync(int? userId = null, CancellationToken cancellationToken = default);

    // Goal progress updates
    Task UpdateGoalProgressAsync(int goalId, decimal newAmount, CancellationToken cancellationToken = default);

    // Goal validation
    Task<bool> ValidateGoalAsync(Goal goal, CancellationToken cancellationToken = default);

    // Goal statistics
    Task<int> GetCompletedGoalsCountAsync(int? userId = null, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalGoalAmountAsync(int? userId = null, CancellationToken cancellationToken = default);
}