using minimal_api.Dominio.Entidades;

namespace minimal_api.Dominio.Interfaces;

/// <summary>
/// Specialized Category Repository interface
/// Supports category management and validation
/// </summary>
public interface ICategoryRepository : IRepository<Category>
{
    // Active Categories (for dropdowns and selections)
    Task<IEnumerable<Category>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default);

    // Find by name for validation
    Task<Category?> FindByNameAsync(string name, CancellationToken cancellationToken = default);

    // Category usage statistics
    Task<Dictionary<int, int>> GetCategoryUsageCountAsync(CancellationToken cancellationToken = default);

    // Validate category data
    Task<bool> ValidateCategoryAsync(Category category, CancellationToken cancellationToken = default);
}