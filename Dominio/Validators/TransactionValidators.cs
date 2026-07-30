using FluentValidation;
using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Interfaces;

namespace minimal_api.Dominio.Validators;

/// <summary>
/// Validador para CreateTransactionDto usando FluentValidation
/// Implementa validações avançadas além das Data Annotations
/// Requirements 1: Validações de transação
/// </summary>
public class CreateTransactionDtoValidator : AbstractValidator<CreateTransactionDto>
{
    private readonly ICategoryRepository _categoryRepository;

    public CreateTransactionDtoValidator(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;

        // Validação de valor - Requirement 1.2
        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("O valor deve ser maior que zero")
            .LessThanOrEqualTo(999999999.99m)
            .WithMessage("O valor não pode exceder R$ 999.999.999,99");

        // Validação de data - Requirement 1.3
        RuleFor(x => x.Date)
            .NotEmpty()
            .WithMessage("A data é obrigatória")
            .Must(BeValidDate)
            .WithMessage("A data não pode ser futura")
            .GreaterThan(new DateTime(2020, 1, 1))
            .WithMessage("A data deve ser posterior a 01/01/2020");

        // Validação de tipo
        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Tipo de transação inválido");

        // Validação de categoria - Requirement 1.5
        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .WithMessage("ID da categoria deve ser válido")
            .MustAsync(CategoryExists)
            .WithMessage("A categoria selecionada não existe");

        // Validação de descrição - Requirement 1.6
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("A descrição é obrigatória")
            .Length(3, 255)
            .WithMessage("A descrição deve ter entre 3 e 255 caracteres")
            .Must(NotContainInvalidCharacters)
            .WithMessage("A descrição contém caracteres inválidos");

        // Validação de usuário (quando implementado)
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .When(x => x.UserId.HasValue)
            .WithMessage("ID do usuário deve ser válido quando informado");
    }

    /// <summary>
    /// Valida se a data não é futura
    /// Requirement 1.3: data não pode ultrapassar data atual
    /// </summary>
    private static bool BeValidDate(DateTime date)
    {
        // Permite até o final do dia atual para evitar problemas de timezone
        return date <= DateTime.Now.Date.AddDays(1).AddTicks(-1);
    }

    /// <summary>
    /// Valida se a categoria existe no sistema
    /// Requirement 1.5: validar contra categorias predefinidas
    /// </summary>
    private async Task<bool> CategoryExists(int categoryId, CancellationToken cancellationToken)
    {
        try
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken);
            return category != null;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Valida se a descrição não contém caracteres especiais prejudiciais
    /// Requirement 1.6: sem caracteres especiais prejudiciais
    /// </summary>
    private static bool NotContainInvalidCharacters(string description)
    {
        if (string.IsNullOrEmpty(description))
            return false;

        // Lista de caracteres potencialmente perigosos
        var invalidChars = new[] { '<', '>', '"', '\'', '&', '\0', '\r', '\n' };
        
        return !description.Any(c => invalidChars.Contains(c));
    }
}

/// <summary>
/// Validador para UpdateTransactionDto
/// Implementa Requirements 7: Edição de Transações
/// </summary>
public class UpdateTransactionDtoValidator : AbstractValidator<UpdateTransactionDto>
{
    private readonly ICategoryRepository _categoryRepository;

    public UpdateTransactionDtoValidator(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;

        // Validação de ID - Requirement 7.4
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("ID da transação deve ser válido");

        // Reutilizar validações do CreateTransactionDto
        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("O valor deve ser maior que zero")
            .LessThanOrEqualTo(999999999.99m)
            .WithMessage("O valor não pode exceder R$ 999.999.999,99");

        RuleFor(x => x.Date)
            .NotEmpty()
            .WithMessage("A data é obrigatória")
            .Must(BeValidDate)
            .WithMessage("A data não pode ser futura")
            .GreaterThan(new DateTime(2020, 1, 1))
            .WithMessage("A data deve ser posterior a 01/01/2020");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Tipo de transação inválido");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .WithMessage("ID da categoria deve ser válido")
            .MustAsync(CategoryExists)
            .WithMessage("A categoria selecionada não existe");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("A descrição é obrigatória")
            .Length(3, 255)
            .WithMessage("A descrição deve ter entre 3 e 255 caracteres")
            .Must(NotContainInvalidCharacters)
            .WithMessage("A descrição contém caracteres inválidos");
    }

    private static bool BeValidDate(DateTime date)
    {
        return date <= DateTime.Now.Date.AddDays(1).AddTicks(-1);
    }

    private async Task<bool> CategoryExists(int categoryId, CancellationToken cancellationToken)
    {
        try
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken);
            return category != null;
        }
        catch
        {
            return false;
        }
    }

    private static bool NotContainInvalidCharacters(string description)
    {
        if (string.IsNullOrEmpty(description))
            return false;

        var invalidChars = new[] { '<', '>', '"', '\'', '&', '\0', '\r', '\n' };
        return !description.Any(c => invalidChars.Contains(c));
    }
}

/// <summary>
/// Validador para TransactionFilterDto
/// Implementa Requirements 2, 3, 4: Paginação e Filtros
/// </summary>
public class TransactionFilterDtoValidator : AbstractValidator<TransactionFilterDto>
{
    private readonly ICategoryRepository _categoryRepository;

    public TransactionFilterDtoValidator(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;

        // Validação de paginação - Requirement 2
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("A página deve ser maior que zero")
            .LessThanOrEqualTo(10000)
            .WithMessage("Número da página muito alto");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("O tamanho da página deve estar entre 1 e 100");

        // Validação de período - Requirement 3.2
        RuleFor(x => x)
            .Must(x => x.IsDateRangeValid())
            .WithMessage("A data de início deve ser anterior ou igual à data de fim")
            .WithName("Período");

        // Validação de datas individuais
        RuleFor(x => x.StartDate)
            .LessThanOrEqualTo(DateTime.Now.Date.AddDays(1))
            .When(x => x.StartDate.HasValue)
            .WithMessage("A data de início não pode ser futura");

        RuleFor(x => x.EndDate)
            .LessThanOrEqualTo(DateTime.Now.Date.AddDays(1))
            .When(x => x.EndDate.HasValue)
            .WithMessage("A data de fim não pode ser futura");

        // Validação de período máximo (evitar consultas muito pesadas)
        RuleFor(x => x)
            .Must(x => IsValidDateRange(x.StartDate, x.EndDate))
            .WithMessage("O período não pode ser superior a 2 anos")
            .WithName("Período");

        // Validação de categorias - Requirement 4.4
        RuleFor(x => x.CategoryIds)
            .Must(BeValidCategoryList)
            .WithMessage("Lista de categorias inválida")
            .MustAsync(AllCategoriesExist)
            .When(x => x.CategoryIds?.Any() == true)
            .WithMessage("Uma ou mais categorias selecionadas não existem");

        // Validação de tipo
        RuleFor(x => x.Type)
            .IsInEnum()
            .When(x => x.Type.HasValue)
            .WithMessage("Tipo de transação inválido");

        // Validação de usuário
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .When(x => x.UserId.HasValue)
            .WithMessage("ID do usuário deve ser válido quando informado");
    }

    /// <summary>
    /// Valida se o período não é muito extenso
    /// Evita consultas muito pesadas
    /// </summary>
    private static bool IsValidDateRange(DateTime? startDate, DateTime? endDate)
    {
        if (!startDate.HasValue || !endDate.HasValue)
            return true; // Será aplicado default

        var difference = endDate.Value - startDate.Value;
        return difference.TotalDays <= 730; // Máximo 2 anos
    }

    /// <summary>
    /// Valida se a lista de categorias é válida
    /// </summary>
    private static bool BeValidCategoryList(List<int> categoryIds)
    {
        if (categoryIds == null || !categoryIds.Any())
            return true;

        // Verificar se todos são positivos e únicos
        return categoryIds.All(id => id > 0) && 
               categoryIds.Count == categoryIds.Distinct().Count();
    }

    /// <summary>
    /// Valida se todas as categorias existem
    /// Requirement 4.4: validar contra categorias predefinidas
    /// </summary>
    private async Task<bool> AllCategoriesExist(List<int> categoryIds, CancellationToken cancellationToken)
    {
        if (categoryIds == null || !categoryIds.Any())
            return true;

        try
        {
            foreach (var categoryId in categoryIds)
            {
                var category = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken);
                if (category == null)
                    return false;
            }
            return true;
        }
        catch
        {
            return false;
        }
    }
}

/// <summary>
/// Simple CreateTransaction validator for unit testing (without database dependencies)
/// </summary>
public class SimpleCreateTransactionValidator : AbstractValidator<CreateTransactionDto>
{
    public SimpleCreateTransactionValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("O valor deve ser maior que zero");

        RuleFor(x => x.Date)
            .NotEmpty()
            .WithMessage("A data é obrigatória")
            .Must(BeValidDate)
            .WithMessage("A data não pode ser futura");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Tipo de transação inválido");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .WithMessage("ID da categoria deve ser válido");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("A descrição é obrigatória")
            .Length(3, 255)
            .WithMessage("A descrição deve ter entre 3 e 255 caracteres");
    }

    private static bool BeValidDate(DateTime date)
    {
        return date <= DateTime.Now.Date.AddDays(1).AddTicks(-1);
    }
}

/// <summary>
/// Simple UpdateTransaction validator for unit testing (without database dependencies)
/// </summary>
public class SimpleUpdateTransactionValidator : AbstractValidator<UpdateTransactionDto>
{
    public SimpleUpdateTransactionValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("ID da transação deve ser válido");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("O valor deve ser maior que zero");

        RuleFor(x => x.Date)
            .NotEmpty()
            .WithMessage("A data é obrigatória")
            .Must(BeValidDate)
            .WithMessage("A data não pode ser futura");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Tipo de transação inválido");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .WithMessage("ID da categoria deve ser válido");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("A descrição é obrigatória")
            .Length(3, 255)
            .WithMessage("A descrição deve ter entre 3 e 255 caracteres");
    }

    private static bool BeValidDate(DateTime date)
    {
        return date <= DateTime.Now.Date.AddDays(1).AddTicks(-1);
    }
}

/// <summary>
/// Simple TransactionFilter validator for unit testing (without database dependencies)
/// </summary>
public class SimpleTransactionFilterValidator : AbstractValidator<TransactionFilterDto>
{
    public SimpleTransactionFilterValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("A página deve ser maior que zero");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("O tamanho da página deve estar entre 1 e 100");

        RuleFor(x => x.StartDate)
            .LessThanOrEqualTo(x => x.EndDate)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
            .WithMessage("A data de início deve ser anterior ou igual à data de fim");

        RuleFor(x => x.Type)
            .IsInEnum()
            .When(x => x.Type.HasValue)
            .WithMessage("Tipo de transação inválido");
    }
}