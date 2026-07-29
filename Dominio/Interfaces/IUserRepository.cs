using minimal_api.Dominio.Entidades;

namespace minimal_api.Dominio.Interfaces;

/// <summary>
/// Specialized User Repository interface
/// Supports user authentication and management (Requirement 17)
/// </summary>
public interface IUserRepository : IRepository<User>
{
    // Authentication support
    Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);

    // User validation
    Task<bool> ValidateUserAsync(User user, CancellationToken cancellationToken = default);
    Task<bool> IsEmailUniqueAsync(string email, int? excludeUserId = null, CancellationToken cancellationToken = default);

    // User statistics
    Task<int> GetActiveUsersCountAsync(CancellationToken cancellationToken = default);
}