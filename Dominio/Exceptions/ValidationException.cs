using System.Collections.Generic;
using System.Linq;

namespace minimal_api.Dominio.Exceptions;

/// <summary>
/// Exception thrown when validation errors occur during transaction processing
/// Supports Requirements 1 and 8 - Transaction validation and error handling
/// </summary>
public class ValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException() : base("Validation failed")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(string message) : base(message)
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(string message, Exception innerException) : base(message, innerException)
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IDictionary<string, string[]> errors) : base("Validation failed")
    {
        Errors = errors;
    }

    public ValidationException(string field, string error) : base($"Validation failed for field '{field}': {error}")
    {
        Errors = new Dictionary<string, string[]>
        {
            [field] = new[] { error }
        };
    }

    public ValidationException(string field, string[] errors) : base($"Validation failed for field '{field}'")
    {
        Errors = new Dictionary<string, string[]>
        {
            [field] = errors
        };
    }

    /// <summary>
    /// Creates a ValidationException for invalid transaction value (Requirements 1.2)
    /// </summary>
    public static ValidationException ForInvalidValue(decimal value)
    {
        return new ValidationException("Valor", new[] { $"O valor deve ser maior que zero. Valor informado: {value:C}" });
    }

    /// <summary>
    /// Creates a ValidationException for future date (Requirements 1.3)
    /// </summary>
    public static ValidationException ForFutureDate(DateTime date)
    {
        return new ValidationException("Data", new[] { $"A data não pode ser futura. Data informada: {date:dd/MM/yyyy}" });
    }

    /// <summary>
    /// Creates a ValidationException for missing required fields (Requirements 1.1)
    /// </summary>
    public static ValidationException ForMissingFields(params string[] fields)
    {
        var errors = new Dictionary<string, string[]>();
        foreach (var field in fields)
        {
            errors[field] = new[] { $"O campo {field} é obrigatório" };
        }
        return new ValidationException(errors);
    }

    /// <summary>
    /// Creates a ValidationException for invalid category (Requirements 1.5)
    /// </summary>
    public static ValidationException ForInvalidCategory(string category)
    {
        return new ValidationException("Categoria", new[] { $"Categoria '{category}' não é válida" });
    }

    /// <summary>
    /// Creates a ValidationException for description length (Requirements 1.6)
    /// </summary>
    public static ValidationException ForInvalidDescription(string description)
    {
        return new ValidationException("Descricao", new[] { $"A descrição deve ter no máximo 255 caracteres. Atual: {description?.Length ?? 0}" });
    }

    /// <summary>
    /// Creates a ValidationException for invalid date range in filters (Requirements 3.2)
    /// </summary>
    public static ValidationException ForInvalidDateRange(DateTime startDate, DateTime endDate)
    {
        return new ValidationException("Periodo", new[] { $"A data inicial ({startDate:dd/MM/yyyy}) não pode ser posterior à data final ({endDate:dd/MM/yyyy})" });
    }

    public override string ToString()
    {
        if (Errors.Any())
        {
            var errorMessages = Errors.SelectMany(e => e.Value.Select(v => $"{e.Key}: {v}"));
            return $"{Message}\nErros:\n{string.Join("\n", errorMessages)}";
        }
        return base.ToString();
    }
}