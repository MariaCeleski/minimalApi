namespace minimal_api.Dominio.Exceptions;

/// <summary>
/// Exception thrown when a requested resource is not found
/// Supports Requirements 7 and 8 - Transaction editing and deletion error handling
/// </summary>
public class NotFoundException : Exception
{
    public string ResourceType { get; }
    public object ResourceId { get; }

    public NotFoundException() : base("Resource not found")
    {
        ResourceType = "Resource";
        ResourceId = string.Empty;
    }

    public NotFoundException(string message) : base(message)
    {
        ResourceType = "Resource";
        ResourceId = string.Empty;
    }

    public NotFoundException(string message, Exception innerException) : base(message, innerException)
    {
        ResourceType = "Resource";
        ResourceId = string.Empty;
    }

    public NotFoundException(string resourceType, object resourceId) 
        : base($"{resourceType} with ID '{resourceId}' was not found")
    {
        ResourceType = resourceType;
        ResourceId = resourceId;
    }

    public NotFoundException(string resourceType, object resourceId, string additionalMessage) 
        : base($"{resourceType} with ID '{resourceId}' was not found. {additionalMessage}")
    {
        ResourceType = resourceType;
        ResourceId = resourceId;
    }

    /// <summary>
    /// Creates a NotFoundException for transaction not found (Requirements 7.4, 8.1)
    /// </summary>
    public static NotFoundException ForTransaction(int transactionId)
    {
        return new NotFoundException("Transação", transactionId);
    }

    /// <summary>
    /// Creates a NotFoundException for category not found (Requirements 1.5)
    /// </summary>
    public static NotFoundException ForCategory(int categoryId)
    {
        return new NotFoundException("Categoria", categoryId);
    }

    /// <summary>
    /// Creates a NotFoundException for user not found (Requirements 17)
    /// </summary>
    public static NotFoundException ForUser(int userId)
    {
        return new NotFoundException("Usuário", userId);
    }

    /// <summary>
    /// Creates a NotFoundException for goal not found (Requirements 18)
    /// </summary>
    public static NotFoundException ForGoal(int goalId)
    {
        return new NotFoundException("Meta", goalId);
    }

    /// <summary>
    /// Creates a NotFoundException for transaction limit not found (Requirements 19)
    /// </summary>
    public static NotFoundException ForTransactionLimit(int limitId)
    {
        return new NotFoundException("Limite", limitId);
    }

    /// <summary>
    /// Creates a NotFoundException for transaction limit by category (Requirements 19)
    /// </summary>
    public static NotFoundException ForTransactionLimitByCategory(int categoryId)
    {
        return new NotFoundException("Limite", categoryId, "Não existe limite definido para esta categoria.");
    }
}