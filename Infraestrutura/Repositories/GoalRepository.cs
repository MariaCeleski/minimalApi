using Microsoft.EntityFrameworkCore;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Interfaces;
using minimal_api.Infraestrutura.Db;

namespace minimal_api.Infraestrutura.Repositories;

/// <summary>
/// Specialized Goal Repository implementation
/// Supports financial goal management (Requirement 18)
/// </summary>
public class GoalRepository : Repository<Goal>, IGoalRepository
{
    public GoalRepository(DbContexto context) : base(context)
    {
    }

    public override async Task<Goal?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(g => g.User)
            .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Goal>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(g => g.User)
            .Where(g => g.UserId == userId)
            .OrderBy(g => g.TargetDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<Goal>> GetByUserIdAsync(
        int userId,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        return await GetPagedAsync(
            filter: g => g.UserId == userId,
            orderBy: q => q.OrderBy(g => g.TargetDate),
            page: page,
            pageSize: pageSize,
            cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<Goal>> GetByStatusAsync(
        GoalStatus status,
        int? userId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(g => g.User)
            .Where(g => g.Status == status);

        if (userId.HasValue)
        {
            query = query.Where(g => g.UserId == userId.Value);
        }

        return await query
            .OrderBy(g => g.TargetDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Goal>> GetActiveGoalsAsync(
        int? userId = null,
        CancellationToken cancellationToken = default)
    {
        return await GetByStatusAsync(GoalStatus.Active, userId, cancellationToken);
    }

    public async Task UpdateGoalProgressAsync(
        int goalId,
        decimal newAmount,
        CancellationToken cancellationToken = default)
    {
        var goal = await _dbSet.FindAsync([goalId], cancellationToken);
        if (goal == null)
        {
            throw new InvalidOperationException($"Goal with ID {goalId} not found");
        }

        goal.CurrentAmount = newAmount;
        goal.UpdatedAt = DateTime.UtcNow;

        // Update status if goal is completed
        if (newAmount >= goal.TargetAmount && goal.Status == GoalStatus.Active)
        {
            goal.Status = GoalStatus.Completed;
        }
        // Reactivate goal if it was completed but amount is now less than target
        else if (newAmount < goal.TargetAmount && goal.Status == GoalStatus.Completed)
        {
            goal.Status = GoalStatus.Active;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ValidateGoalAsync(Goal goal, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(goal);

        // Validate required fields
        if (string.IsNullOrWhiteSpace(goal.Name))
            return false;

        if (goal.TargetAmount <= 0)
            return false;

        if (goal.CurrentAmount < 0)
            return false;

        if (goal.TargetDate <= DateTime.UtcNow.Date)
            return false;

        if (goal.UserId <= 0)
            return false;

        // Validate user exists
        var userExists = await _context.Users
            .AnyAsync(u => u.Id == goal.UserId, cancellationToken);

        if (!userExists)
            return false;

        return true;
    }

    public async Task<int> GetCompletedGoalsCountAsync(
        int? userId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(g => g.Status == GoalStatus.Completed);

        if (userId.HasValue)
        {
            query = query.Where(g => g.UserId == userId.Value);
        }

        return await query.CountAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalGoalAmountAsync(
        int? userId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(g => g.Status == GoalStatus.Active);

        if (userId.HasValue)
        {
            query = query.Where(g => g.UserId == userId.Value);
        }

        return await query.SumAsync(g => g.TargetAmount, cancellationToken);
    }
}