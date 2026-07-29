using Microsoft.EntityFrameworkCore;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Interfaces;
using minimal_api.Infraestrutura.Db;

namespace minimal_api.Infraestrutura.Repositories;

/// <summary>
/// Specialized Transaction Repository with filtering and balance calculation capabilities
/// Supports Requirements 1, 2, 3, 4, 5, 6
/// </summary>
public class TransactionRepository : Repository<Transaction>, ITransactionRepository
{
    public TransactionRepository(DbContexto context) : base(context)
    {
    }

    public override async Task<Transaction?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.Category)
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public override async Task<IEnumerable<Transaction>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.Category)
            .Include(t => t.User)
            .OrderByDescending(t => t.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<Transaction>> GetByPeriodAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        // Apply default period filtering (30 days back if not specified) - Requirement 3
        startDate ??= DateTime.UtcNow.AddDays(-30);
        endDate ??= DateTime.UtcNow;

        // Validate date range (Requirement 3)
        if (startDate > endDate)
        {
            throw new ArgumentException("Start date cannot be after end date", nameof(startDate));
        }

        return await GetPagedAsync(
            filter: t => t.Date >= startDate && t.Date <= endDate,
            orderBy: q => q.OrderByDescending(t => t.Date),
            page: page,
            pageSize: pageSize,
            cancellationToken: cancellationToken);
    }

    public async Task<PagedResult<Transaction>> GetByCategoriesAsync(
        int[] categoryIds,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(categoryIds);

        if (categoryIds.Length == 0)
        {
            return await GetPagedAsync(
                page: page,
                pageSize: pageSize,
                cancellationToken: cancellationToken);
        }

        return await GetPagedAsync(
            filter: t => categoryIds.Contains(t.CategoryId),
            orderBy: q => q.OrderByDescending(t => t.Date),
            page: page,
            pageSize: pageSize,
            cancellationToken: cancellationToken);
    }

    public async Task<PagedResult<Transaction>> GetFilteredAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        int[]? categoryIds = null,
        int? userId = null,
        TransactionType? type = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        // Apply defaults
        startDate ??= DateTime.UtcNow.AddDays(-30);
        endDate ??= DateTime.UtcNow;

        if (startDate > endDate)
        {
            throw new ArgumentException("Start date cannot be after end date", nameof(startDate));
        }

        var query = _dbSet
            .Include(t => t.Category)
            .Include(t => t.User)
            .Where(t => t.Date >= startDate && t.Date <= endDate);

        if (categoryIds?.Length > 0)
            query = query.Where(t => categoryIds.Contains(t.CategoryId));

        if (userId.HasValue)
            query = query.Where(t => t.UserId == userId);

        if (type.HasValue)
            query = query.Where(t => t.Type == type);

        // Manual pagination to avoid method call issues
        var totalItems = await query.CountAsync(cancellationToken);
        var skip = (page - 1) * pageSize;
        var data = await query
            .OrderByDescending(t => t.Date)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        return new PagedResult<Transaction>
        {
            Data = data,
            CurrentPage = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages
        };
    }

    public async Task<decimal> CalculateBalanceAsync(
        int? userId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();

        if (userId.HasValue)
            query = query.Where(t => t.UserId == userId);

        var income = await query
            .Where(t => t.Type == TransactionType.Income)
            .SumAsync(t => t.Amount, cancellationToken);

        var expenses = await query
            .Where(t => t.Type == TransactionType.Expense)
            .SumAsync(t => t.Amount, cancellationToken);

        return income - expenses;
    }

    public async Task<decimal> CalculateBalanceForPeriodAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        int? userId = null,
        CancellationToken cancellationToken = default)
    {
        startDate ??= DateTime.UtcNow.AddDays(-30);
        endDate ??= DateTime.UtcNow;

        var query = _dbSet
            .Where(t => t.Date >= startDate && t.Date <= endDate);

        if (userId.HasValue)
            query = query.Where(t => t.UserId == userId);

        var income = await query
            .Where(t => t.Type == TransactionType.Income)
            .SumAsync(t => t.Amount, cancellationToken);

        var expenses = await query
            .Where(t => t.Type == TransactionType.Expense)
            .SumAsync(t => t.Amount, cancellationToken);

        return income - expenses;
    }

    public async Task<decimal> CalculateIncomeAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        int? userId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(t => t.Type == TransactionType.Income);

        if (startDate.HasValue)
            query = query.Where(t => t.Date >= startDate);

        if (endDate.HasValue)
            query = query.Where(t => t.Date <= endDate);

        if (userId.HasValue)
            query = query.Where(t => t.UserId == userId);

        return await query.SumAsync(t => t.Amount, cancellationToken);
    }

    public async Task<decimal> CalculateExpenseAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        int? userId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(t => t.Type == TransactionType.Expense);

        if (startDate.HasValue)
            query = query.Where(t => t.Date >= startDate);

        if (endDate.HasValue)
            query = query.Where(t => t.Date <= endDate);

        if (userId.HasValue)
            query = query.Where(t => t.UserId == userId);

        return await query.SumAsync(t => t.Amount, cancellationToken);
    }

    public async Task<Dictionary<int, decimal>> GetCategoryDistributionAsync(
        TransactionType type,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int? userId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(t => t.Type == type);

        if (startDate.HasValue)
            query = query.Where(t => t.Date >= startDate);

        if (endDate.HasValue)
            query = query.Where(t => t.Date <= endDate);

        if (userId.HasValue)
            query = query.Where(t => t.UserId == userId);

        return await query
            .GroupBy(t => t.CategoryId)
            .Select(g => new { CategoryId = g.Key, Total = g.Sum(t => t.Amount) })
            .ToDictionaryAsync(x => x.CategoryId, x => x.Total, cancellationToken);
    }

    public async Task<Dictionary<DateTime, decimal>> GetMonthlyTrendAsync(
        int monthsBack = 12,
        int? userId = null,
        CancellationToken cancellationToken = default)
    {
        var startDate = DateTime.UtcNow.AddMonths(-monthsBack);
        var query = _dbSet
            .Where(t => t.Date >= startDate);

        if (userId.HasValue)
            query = query.Where(t => t.UserId == userId);

        var monthlyData = await query
            .GroupBy(t => new { t.Date.Year, t.Date.Month })
            .Select(g => new {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Income = g.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount),
                Expenses = g.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount)
            })
            .ToListAsync(cancellationToken);

        return monthlyData.ToDictionary(
            x => new DateTime(x.Year, x.Month, 1),
            x => x.Income - x.Expenses);
    }

    /// <summary>
    /// Get detailed monthly trend data with income and expenses separated
    /// Task 3.8: Create MonthlyTrend DTO e endpoint
    /// Returns Dictionary with Month as key and tuple of (Income, Expenses, Balance) as value
    /// </summary>
    public async Task<Dictionary<DateTime, (decimal Income, decimal Expenses, decimal Balance)>> GetMonthlyTrendDetailedAsync(
        int monthsBack = 12,
        int? userId = null,
        CancellationToken cancellationToken = default)
    {
        var startDate = DateTime.UtcNow.AddMonths(-monthsBack);
        var query = _dbSet
            .Where(t => t.Date >= startDate);

        if (userId.HasValue)
            query = query.Where(t => t.UserId == userId);

        var monthlyData = await query
            .GroupBy(t => new { t.Date.Year, t.Date.Month })
            .Select(g => new {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Income = g.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount),
                Expenses = g.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount)
            })
            .ToListAsync(cancellationToken);

        return monthlyData.ToDictionary(
            x => new DateTime(x.Year, x.Month, 1),
            x => (x.Income, x.Expenses, x.Income - x.Expenses));
    }

    public async Task<bool> ValidateTransactionAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(transaction);

        // Validate amount is positive
        if (transaction.Amount <= 0)
            return false;

        // Validate date is not future
        if (transaction.Date > DateTime.Now.Date.AddDays(1))
            return false;

        // Validate category exists
        var categoryExists = await _context.Categories
            .AnyAsync(c => c.Id == transaction.CategoryId, cancellationToken);

        if (!categoryExists)
            return false;

        // Validate user exists if UserId is provided
        if (transaction.UserId.HasValue)
        {
            var userExists = await _context.Users
                .AnyAsync(u => u.Id == transaction.UserId.Value, cancellationToken);

            if (!userExists)
                return false;
        }

        return true;
    }

    // Additional methods for TransactionService
    public async Task<PagedResult<Transaction>> GetPagedTransactionsAsync(
        int page,
        int pageSize,
        DateTime? startDate = null,
        DateTime? endDate = null,
        List<int>? categoryIds = null,
        TransactionType? type = null,
        int? userId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(t => t.Category)
            .Include(t => t.User)
            .AsQueryable();

        // Apply filters
        if (startDate.HasValue)
            query = query.Where(t => t.Date >= startDate);

        if (endDate.HasValue)
            query = query.Where(t => t.Date <= endDate);

        if (categoryIds?.Count > 0)
            query = query.Where(t => categoryIds.Contains(t.CategoryId));

        if (type.HasValue)
            query = query.Where(t => t.Type == type);

        if (userId.HasValue)
            query = query.Where(t => t.UserId == userId);

        // Manual pagination
        var totalItems = await query.CountAsync(cancellationToken);
        var skip = (page - 1) * pageSize;
        var data = await query
            .OrderByDescending(t => t.Date)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        return new PagedResult<Transaction>
        {
            Data = data,
            CurrentPage = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages
        };
    }

    public async Task<decimal> GetTotalByTypeAsync(
        TransactionType type,
        int? userId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(t => t.Type == type);

        if (userId.HasValue)
            query = query.Where(t => t.UserId == userId);

        return await query.SumAsync(t => t.Amount, cancellationToken);
    }

    public async Task<decimal> GetTotalByTypeInPeriodAsync(
        TransactionType type,
        DateTime startDate,
        DateTime endDate,
        int? userId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(t => t.Type == type && t.Date >= startDate && t.Date <= endDate);

        if (userId.HasValue)
            query = query.Where(t => t.UserId == userId);

        return await query.SumAsync(t => t.Amount, cancellationToken);
    }

    public async Task<int> GetCountInPeriodAsync(
        DateTime startDate,
        DateTime endDate,
        int? userId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(t => t.Date >= startDate && t.Date <= endDate);

        if (userId.HasValue)
            query = query.Where(t => t.UserId == userId);

        return await query.CountAsync(cancellationToken);
    }
}