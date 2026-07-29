using Microsoft.EntityFrameworkCore;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Interfaces;
using minimal_api.Infraestrutura.Db;

namespace minimal_api.Infraestrutura.Repositories;

/// <summary>
/// Specialized Transaction Limit Repository implementation
/// Supports spending limit management and notifications (Requirement 19)
/// </summary>
public class TransactionLimitRepository : Repository<TransactionLimit>, ITransactionLimitRepository
{
    public TransactionLimitRepository(DbContexto context) : base(context)
    {
    }

    public override async Task<TransactionLimit?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(tl => tl.Category)
            .Include(tl => tl.User)
            .FirstOrDefaultAsync(tl => tl.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<TransactionLimit>> GetByUserIdAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(tl => tl.Category)
            .Where(tl => tl.UserId == userId)
            .OrderBy(tl => tl.Category.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TransactionLimit>> GetByCategoryIdAsync(
        int categoryId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(tl => tl.User)
            .Include(tl => tl.Category)
            .Where(tl => tl.CategoryId == categoryId)
            .ToListAsync(cancellationToken);
    }

    public async Task<TransactionLimit?> FindByCategoryAndUserAsync(
        int categoryId,
        int? userId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(tl => tl.Category)
            .Include(tl => tl.User)
            .Where(tl => tl.CategoryId == categoryId);

        if (userId.HasValue)
        {
            query = query.Where(tl => tl.UserId == userId.Value);
        }
        else
        {
            query = query.Where(tl => tl.UserId == null);
        }

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task UpdateCurrentSpentAsync(
        int limitId,
        decimal newAmount,
        CancellationToken cancellationToken = default)
    {
        var limit = await _dbSet.FindAsync([limitId], cancellationToken);
        if (limit == null)
        {
            throw new InvalidOperationException($"Transaction limit with ID {limitId} not found");
        }

        limit.CurrentSpent = newAmount;
        limit.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<decimal> CalculateCurrentSpentAsync(
        int categoryId,
        LimitPeriod period,
        int? userId = null,
        CancellationToken cancellationToken = default)
    {
        var (startDate, endDate) = GetPeriodDates(period);

        var query = _context.Transactions
            .Where(t => t.CategoryId == categoryId &&
                       t.Type == TransactionType.Expense &&
                       t.Date >= startDate &&
                       t.Date <= endDate);

        if (userId.HasValue)
        {
            query = query.Where(t => t.UserId == userId.Value);
        }

        return await query.SumAsync(t => t.Amount, cancellationToken);
    }

    public async Task<(bool isWarning, bool isExceeded)> CheckLimitStatusAsync(
        int limitId,
        CancellationToken cancellationToken = default)
    {
        var limit = await _dbSet.FindAsync([limitId], cancellationToken);
        if (limit == null)
        {
            throw new InvalidOperationException($"Transaction limit with ID {limitId} not found");
        }

        if (limit.LimitAmount <= 0)
        {
            return (false, false);
        }

        var percentage = (limit.CurrentSpent / limit.LimitAmount) * 100;

        // Requirement 19: Warning at 80%, Alert at 100%
        var isWarning = percentage >= 80 && percentage < 100;
        var isExceeded = percentage >= 100;

        return (isWarning, isExceeded);
    }

    public async Task<IEnumerable<TransactionLimit>> GetLimitsForTransactionAsync(
        int categoryId,
        int? userId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(tl => tl.Category)
            .Include(tl => tl.User)
            .Where(tl => tl.CategoryId == categoryId);

        if (userId.HasValue)
        {
            // Get both user-specific and global limits for this category
            query = query.Where(tl => tl.UserId == userId.Value || tl.UserId == null);
        }
        else
        {
            // Get only global limits
            query = query.Where(tl => tl.UserId == null);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<bool> ValidateLimitAsync(
        TransactionLimit limit,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(limit);

        // Validate required fields
        if (string.IsNullOrWhiteSpace(limit.Name))
            return false;

        if (limit.LimitAmount <= 0)
            return false;

        if (limit.CurrentSpent < 0)
            return false;

        if (limit.CategoryId <= 0)
            return false;

        // Validate category exists
        var categoryExists = await _context.Categories
            .AnyAsync(c => c.Id == limit.CategoryId, cancellationToken);

        if (!categoryExists)
            return false;

        // Validate user exists if UserId is provided
        if (limit.UserId.HasValue)
        {
            var userExists = await _context.Users
                .AnyAsync(u => u.Id == limit.UserId.Value, cancellationToken);

            if (!userExists)
                return false;
        }

        // Check for duplicate limits (same category, user, and period)
        var existingLimit = await _dbSet
            .Where(tl => tl.CategoryId == limit.CategoryId &&
                        tl.UserId == limit.UserId &&
                        tl.Period == limit.Period &&
                        tl.Id != limit.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingLimit != null)
            return false;

        return true;
    }

    public async Task ResetLimitsForPeriodAsync(
        LimitPeriod period,
        CancellationToken cancellationToken = default)
    {
        var limitsToReset = await _dbSet
            .Where(tl => tl.Period == period)
            .ToListAsync(cancellationToken);

        foreach (var limit in limitsToReset)
        {
            limit.CurrentSpent = 0;
            limit.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    private static (DateTime startDate, DateTime endDate) GetPeriodDates(LimitPeriod period)
    {
        var now = DateTime.UtcNow;
        
        return period switch
        {
            LimitPeriod.Daily => (now.Date, now.Date.AddDays(1).AddTicks(-1)),
            LimitPeriod.Weekly => 
            (
                now.Date.AddDays(-(int)now.DayOfWeek), 
                now.Date.AddDays(-(int)now.DayOfWeek).AddDays(7).AddTicks(-1)
            ),
            LimitPeriod.Monthly => 
            (
                new DateTime(now.Year, now.Month, 1), 
                new DateTime(now.Year, now.Month, 1).AddMonths(1).AddTicks(-1)
            ),
            LimitPeriod.Yearly => 
            (
                new DateTime(now.Year, 1, 1), 
                new DateTime(now.Year, 1, 1).AddYears(1).AddTicks(-1)
            ),
            _ => throw new ArgumentOutOfRangeException(nameof(period), period, null)
        };
    }
}