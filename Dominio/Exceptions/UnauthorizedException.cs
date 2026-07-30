namespace minimal_api.Dominio.Exceptions;

/// <summary>
/// Exception thrown when authentication or authorization fails
/// Supports Requirements 17 - Authentication
/// </summary>
public class UnauthorizedException : Exception
{
    public string Action { get; }
    public string Resource { get; }

    public UnauthorizedException() : base("Unauthorized access")
    {
        Action = string.Empty;
        Resource = string.Empty;
    }

    public UnauthorizedException(string message) : base(message)
    {
        Action = string.Empty;
        Resource = string.Empty;
    }

    public UnauthorizedException(string message, Exception innerException) : base(message, innerException)
    {
        Action = string.Empty;
        Resource = string.Empty;
    }

    public UnauthorizedException(string action, string resource) 
        : base($"Unauthorized to perform '{action}' on '{resource}'")
    {
        Action = action;
        Resource = resource;
    }

    /// <summary>
    /// Creates an UnauthorizedException for invalid credentials (Requirements 17.6)
    /// </summary>
    public static UnauthorizedException ForInvalidCredentials()
    {
        return new UnauthorizedException("Credenciais inválidas. Verifique email e senha.");
    }

    /// <summary>
    /// Creates an UnauthorizedException for expired token (Requirements 17.5)
    /// </summary>
    public static UnauthorizedException ForExpiredToken()
    {
        return new UnauthorizedException("Token expirado. Faça login novamente.");
    }

    /// <summary>
    /// Creates an UnauthorizedException for missing token (Requirements 17.5)
    /// </summary>
    public static UnauthorizedException ForMissingToken()
    {
        return new UnauthorizedException("Token de acesso obrigatório. Faça login.");
    }

    /// <summary>
    /// Creates an UnauthorizedException for invalid token (Requirements 17.5)
    /// </summary>
    public static UnauthorizedException ForInvalidToken()
    {
        return new UnauthorizedException("Token inválido. Faça login novamente.");
    }

    /// <summary>
    /// Creates an UnauthorizedException for accessing another user's resource
    /// </summary>
    public static UnauthorizedException ForResourceAccess(string resource, int resourceId)
    {
        return new UnauthorizedException("Acesso", resource, 
            $"Não autorizado a acessar {resource} com ID {resourceId}");
    }

    private UnauthorizedException(string action, string resource, string message) : base(message)
    {
        Action = action;
        Resource = resource;
    }
}