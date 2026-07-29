using Microsoft.EntityFrameworkCore;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Interfaces;
using minimal_api.Infraestrutura.Db;
using System.Text.RegularExpressions;

namespace minimal_api.Infraestrutura.Repositories;

/// <summary>
/// Specialized User Repository implementation
/// Supports user authentication and management (Requirement 17)
/// </summary>
public class UserRepository : Repository<User>, IUserRepository
{
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    public UserRepository(DbContexto context) : base(context)
    {
    }

    public async Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            return null;

        return await _dbSet
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower(), cancellationToken);
    }

    public async Task<bool> ValidateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);

        // Validate required fields
        if (string.IsNullOrWhiteSpace(user.Email))
            return false;

        if (string.IsNullOrWhiteSpace(user.Name))
            return false;

        if (string.IsNullOrWhiteSpace(user.PasswordHash))
            return false;

        // Validate email format (Requirement 17)
        if (!EmailRegex.IsMatch(user.Email))
            return false;

        // Check email uniqueness
        if (!await IsEmailUniqueAsync(user.Email, user.Id, cancellationToken))
            return false;

        return true;
    }

    public async Task<bool> IsEmailUniqueAsync(
        string email,
        int? excludeUserId = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var query = _dbSet.Where(u => u.Email.ToLower() == email.ToLower());

        if (excludeUserId.HasValue)
        {
            query = query.Where(u => u.Id != excludeUserId.Value);
        }

        return !await query.AnyAsync(cancellationToken);
    }

    public async Task<int> GetActiveUsersCountAsync(CancellationToken cancellationToken = default)
    {
        // Count users who have transactions in the last 30 days
        var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);

        return await _dbSet
            .Where(u => u.Transactions.Any(t => t.Date >= thirtyDaysAgo))
            .CountAsync(cancellationToken);
    }
}