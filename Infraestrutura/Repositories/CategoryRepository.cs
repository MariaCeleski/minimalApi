using Microsoft.EntityFrameworkCore;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Interfaces;
using minimal_api.Infraestrutura.Db;

namespace minimal_api.Infraestrutura.Repositories;

/// <summary>
/// Specialized Category Repository implementation
/// Provides category-specific operations and validations
/// </summary>
public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(DbContexto context) : base(context)
    {
    }

    public async Task<IEnumerable<Category>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Category?> FindByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        return await _dbSet
            .FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower(), cancellationToken);
    }

    public async Task<Dictionary<int, int>> GetCategoryUsageCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .GroupBy(t => t.CategoryId)
            .ToDictionaryAsync(
                g => g.Key,
                g => g.Count(),
                cancellationToken);
    }

    public async Task<bool> ValidateCategoryAsync(Category category, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(category);

        // Validate required fields
        if (string.IsNullOrWhiteSpace(category.Name))
            return false;

        if (string.IsNullOrWhiteSpace(category.IconName))
            return false;

        if (string.IsNullOrWhiteSpace(category.Color))
            return false;

        // Validate color format (hex color)
        if (!System.Text.RegularExpressions.Regex.IsMatch(category.Color, @"^#[0-9A-Fa-f]{6}$"))
            return false;

        // Check for duplicate names (excluding current record if updating)
        var existingCategory = await FindByNameAsync(category.Name, cancellationToken);
        if (existingCategory != null && existingCategory.Id != category.Id)
            return false;

        return true;
    }
}