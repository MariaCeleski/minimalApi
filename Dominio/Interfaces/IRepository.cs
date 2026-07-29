using System.Linq.Expressions;

namespace minimal_api.Dominio.Interfaces;

/// <summary>
/// Generic Repository interface providing CRUD operations, pagination, and filtering
/// Supports Requirements 1 (Transaction validation) and 2 (Pagination)
/// </summary>
public interface IRepository<T> where T : class
{
    // Basic CRUD Operations
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    
    // Advanced Query Operations
    Task<IEnumerable<T>> FindAsync(
        Expression<Func<T, bool>> predicate, 
        CancellationToken cancellationToken = default);
    
    Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate, 
        CancellationToken cancellationToken = default);
    
    // Pagination Support (Requirement 2)
    Task<PagedResult<T>> GetPagedAsync(
        int page = 1, 
        int pageSize = 10, 
        CancellationToken cancellationToken = default);
    
    Task<PagedResult<T>> GetPagedAsync(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);
    
    // Count Operations
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
}

/// <summary>
/// Paged result container for repository pagination
/// Contains data and metadata required by Requirement 2
/// </summary>
public class PagedResult<T>
{
    public IEnumerable<T> Data { get; set; } = [];
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
}