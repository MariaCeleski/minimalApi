using FluentValidation;
using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Exceptions;
using minimal_api.Dominio.Interfaces;
using DomainValidationException = minimal_api.Dominio.Exceptions.ValidationException;
using DomainNotFoundException = minimal_api.Dominio.Exceptions.NotFoundException;

namespace minimal_api.Aplicacao.Services;

/// <summary>
/// Serviço para gerenciamento de limites de transações
/// Task 5.6: Implement TransactionLimitService
/// Requirement 19: Notificações de Limite Excedido (Opcional)
/// Permite que usuários definam limites de gastos por categoria
/// </summary>
public class TransactionLimitService : ITransactionLimitService
{
    private readonly ITransactionLimitRepository _limitRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IValidator<CreateTransactionLimitDto> _createValidator;
    private readonly IValidator<UpdateTransactionLimitDto> _updateValidator;

    public TransactionLimitService(
        ITransactionLimitRepository limitRepository,
        ICategoryRepository categoryRepository,
        ITransactionRepository transactionRepository,
        IValidator<CreateTransactionLimitDto> createValidator,
        IValidator<UpdateTransactionLimitDto> updateValidator)
    {
        _limitRepository = limitRepository;
        _categoryRepository = categoryRepository;
        _transactionRepository = transactionRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    /// <summary>
    /// Cria um novo limite de transação
    /// Requirement 19.1: permitir que o usuário defina limite
    /// </summary>
    public async Task<TransactionLimitDto> CreateLimitAsync(CreateTransactionLimitDto dto, CancellationToken cancellationToken = default)
    {
        // Validação com FluentValidation
        var validationResult = await _createValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new DomainValidationException(validationResult.Errors.ToDictionary(
                e => e.PropertyName,
                e => new[] { e.ErrorMessage }));
        }

        // Verificar se categoria existe
        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId, cancellationToken);
        if (category == null)
        {
            throw new DomainNotFoundException("Category", dto.CategoryId);
        }

        // Criar entidade de limite
        var limit = new TransactionLimit
        {
            Name = dto.Name,
            LimitAmount = dto.LimitAmount,
            Period = dto.Period,
            CategoryId = dto.CategoryId,
            UserId = dto.UserId,
            IsActive = true,
            CurrentSpent = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Definir período inicial
        limit.SetCurrentPeriod();

        // Validar limite
        var isValid = await _limitRepository.ValidateLimitAsync(limit, cancellationToken);
        if (!isValid)
        {
            throw new DomainValidationException("Limit", new[] { "Dados do limite são inválidos ou limite duplicado" });
        }

        // Salvar no repositório
        var savedLimit = await _limitRepository.AddAsync(limit, cancellationToken);

        // Calcular gasto atual
        var currentSpent = await _limitRepository.CalculateCurrentSpentAsync(
            savedLimit.CategoryId,
            savedLimit.Period,
            savedLimit.UserId,
            cancellationToken);
        
        savedLimit.CurrentSpent = currentSpent;
        await _limitRepository.UpdateAsync(savedLimit, cancellationToken);

        return MapToDto(savedLimit, category);
    }

    /// <summary>
    /// Obtém um limite específico pelo ID
    /// </summary>
    public async Task<TransactionLimitDto?> GetLimitByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            throw new DomainValidationException("ID", new[] { "ID deve ser maior que zero" });
        }

        var limit = await _limitRepository.GetByIdAsync(id, cancellationToken);
        if (limit == null)
        {
            return null;
        }

        return MapToDto(limit, limit.Category);
    }

    /// <summary>
    /// Lista todos os limites com suporte a paginação e filtros
    /// </summary>
    public async Task<PagedTransactionLimitResponseDto> GetLimitsAsync(TransactionLimitFilterDto filter, CancellationToken cancellationToken = default)
    {
        // Validar filtro
        if (filter.Page < 1)
            filter.Page = 1;
        if (filter.PageSize < 1 || filter.PageSize > 100)
            filter.PageSize = 10;

        // Construir predicate para filtro - começar com um predicate base que sempre é verdadeiro
        System.Linq.Expressions.Expression<System.Func<TransactionLimit, bool>> predicate = l => true;

        if (filter.UserId.HasValue)
        {
            var userId = filter.UserId.Value;
            predicate = l => predicate.Compile()(l) && l.UserId == userId;
        }

        if (filter.CategoryId.HasValue)
        {
            var categoryId = filter.CategoryId.Value;
            predicate = l => predicate.Compile()(l) && l.CategoryId == categoryId;
        }

        if (filter.Period.HasValue)
        {
            var period = filter.Period.Value;
            predicate = l => predicate.Compile()(l) && l.Period == period;
        }

        if (filter.IsActive.HasValue)
        {
            var isActive = filter.IsActive.Value;
            predicate = l => predicate.Compile()(l) && l.IsActive == isActive;
        }

        // Obter dados paginados
        var pagedResult = await _limitRepository.GetPagedAsync(
            filter: predicate,
            orderBy: l => l.OrderBy(x => x.Category.Name),
            page: filter.Page,
            pageSize: filter.PageSize,
            cancellationToken: cancellationToken);

        // Mapear para DTOs
        var limitsDto = pagedResult.Data.Select(l => MapToDto(l, l.Category)).ToList();

        return new PagedTransactionLimitResponseDto
        {
            Data = limitsDto,
            CurrentPage = pagedResult.CurrentPage,
            PageSize = pagedResult.PageSize,
            TotalItems = pagedResult.TotalItems,
            TotalPages = pagedResult.TotalPages
        };
    }

    /// <summary>
    /// Atualiza um limite existente
    /// </summary>
    public async Task<TransactionLimitDto> UpdateLimitAsync(UpdateTransactionLimitDto dto, CancellationToken cancellationToken = default)
    {
        // Validação com FluentValidation
        var validationResult = await _updateValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new DomainValidationException(validationResult.Errors.ToDictionary(
                e => e.PropertyName,
                e => new[] { e.ErrorMessage }));
        }

        // Buscar limite existente
        var limit = await _limitRepository.GetByIdAsync(dto.Id, cancellationToken);
        if (limit == null)
        {
            throw new DomainNotFoundException("TransactionLimit", dto.Id);
        }

        // Verificar se categoria existe
        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId, cancellationToken);
        if (category == null)
        {
            throw new DomainNotFoundException("Category", dto.CategoryId);
        }

        // Atualizar campos
        limit.Name = dto.Name;
        limit.LimitAmount = dto.LimitAmount;
        limit.Period = dto.Period;
        limit.CategoryId = dto.CategoryId;
        limit.IsActive = dto.IsActive;
        limit.UpdatedAt = DateTime.UtcNow;

        // Se o período mudou, resetar data de período
        if (limit.Period != dto.Period)
        {
            limit.SetCurrentPeriod();
            limit.CurrentSpent = 0;
        }

        // Validar limite
        var isValid = await _limitRepository.ValidateLimitAsync(limit, cancellationToken);
        if (!isValid)
        {
            throw new DomainValidationException("Limit", new[] { "Dados do limite são inválidos ou limite duplicado" });
        }

        // Salvar mudanças
        var updatedLimit = await _limitRepository.UpdateAsync(limit, cancellationToken);

        // Recalcular gasto atual
        var currentSpent = await _limitRepository.CalculateCurrentSpentAsync(
            updatedLimit.CategoryId,
            updatedLimit.Period,
            updatedLimit.UserId,
            cancellationToken);
        
        updatedLimit.CurrentSpent = currentSpent;
        await _limitRepository.UpdateAsync(updatedLimit, cancellationToken);

        return MapToDto(updatedLimit, category);
    }

    /// <summary>
    /// Deleta um limite
    /// </summary>
    public async Task<bool> DeleteLimitAsync(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            throw new DomainValidationException("ID", new[] { "ID deve ser maior que zero" });
        }

        var limit = await _limitRepository.GetByIdAsync(id, cancellationToken);
        if (limit == null)
        {
            throw new DomainNotFoundException("TransactionLimit", id);
        }

        return await _limitRepository.DeleteAsync(id, cancellationToken);
    }

    /// <summary>
    /// Retorna um resumo dos limites e notificações
    /// Requirement 19.5: exibir fila de notificações recentes
    /// </summary>
    public async Task<TransactionLimitSummaryDto> GetLimitsSummaryAsync(int? userId = null, CancellationToken cancellationToken = default)
    {
        // Construir filtro
        System.Linq.Expressions.Expression<System.Func<TransactionLimit, bool>> predicate;

        if (userId.HasValue)
        {
            predicate = l => (l.UserId == userId || l.UserId == null);
        }
        else
        {
            predicate = l => l.UserId == null;
        }

        // Obter todos os limites
        var allLimits = (await _limitRepository.GetAllAsync(cancellationToken))
            .Where(predicate.Compile())
            .Where(l => l.IsActive)
            .ToList();

        var warningLimits = new List<TransactionLimitDto>();
        var exceededLimits = new List<TransactionLimitDto>();
        decimal totalSpent = 0;
        decimal totalLimit = 0;

        foreach (var limit in allLimits)
        {
            // Recalcular gasto atual
            var currentSpent = await _limitRepository.CalculateCurrentSpentAsync(
                limit.CategoryId,
                limit.Period,
                limit.UserId,
                cancellationToken);
            
            limit.CurrentSpent = currentSpent;
            totalSpent += currentSpent;
            totalLimit += limit.LimitAmount;

            // Requirement 19.2-19.3: threshold de 80% e 100%
            if (limit.IsExceeded)
            {
                exceededLimits.Add(MapToDto(limit, limit.Category));
            }
            else if (limit.IsNearLimit)
            {
                warningLimits.Add(MapToDto(limit, limit.Category));
            }
        }

        return new TransactionLimitSummaryDto
        {
            TotalLimits = allLimits.Count,
            ActiveLimits = allLimits.Count(l => l.IsActive),
            WarningCount = warningLimits.Count,
            ExceededCount = exceededLimits.Count,
            WarningLimits = warningLimits,
            ExceededLimits = exceededLimits,
            TotalSpent = totalSpent,
            TotalLimit = totalLimit
        };
    }

    /// <summary>
    /// Verifica e atualiza o status de gastos para um limite específico
    /// Requirement 19.2-19.3: verificar se gasto excede 80% e 100%
    /// </summary>
    public async Task<TransactionLimitDto> VerifyAndUpdateLimitStatusAsync(int limitId, CancellationToken cancellationToken = default)
    {
        var limit = await _limitRepository.GetByIdAsync(limitId, cancellationToken);
        if (limit == null)
        {
            throw new DomainNotFoundException("TransactionLimit", limitId);
        }

        // Recalcular gasto atual
        var currentSpent = await _limitRepository.CalculateCurrentSpentAsync(
            limit.CategoryId,
            limit.Period,
            limit.UserId,
            cancellationToken);
        
        limit.CurrentSpent = currentSpent;
        await _limitRepository.UpdateAsync(limit, cancellationToken);

        return MapToDto(limit, limit.Category);
    }

    /// <summary>
    /// Ativa um limite desativado
    /// </summary>
    public async Task<TransactionLimitDto> ActivateLimitAsync(int id, CancellationToken cancellationToken = default)
    {
        var limit = await _limitRepository.GetByIdAsync(id, cancellationToken);
        if (limit == null)
        {
            throw new DomainNotFoundException("TransactionLimit", id);
        }

        limit.Activate();
        var updatedLimit = await _limitRepository.UpdateAsync(limit, cancellationToken);

        return MapToDto(updatedLimit, updatedLimit.Category);
    }

    /// <summary>
    /// Desativa um limite
    /// </summary>
    public async Task<TransactionLimitDto> DeactivateLimitAsync(int id, CancellationToken cancellationToken = default)
    {
        var limit = await _limitRepository.GetByIdAsync(id, cancellationToken);
        if (limit == null)
        {
            throw new DomainNotFoundException("TransactionLimit", id);
        }

        limit.Deactivate();
        var updatedLimit = await _limitRepository.UpdateAsync(limit, cancellationToken);

        return MapToDto(updatedLimit, updatedLimit.Category);
    }

    /// <summary>
    /// Mapeia entidade de limite para DTO
    /// </summary>
    private static TransactionLimitDto MapToDto(TransactionLimit limit, Category category)
    {
        return new TransactionLimitDto
        {
            Id = limit.Id,
            Name = limit.Name,
            LimitAmount = limit.LimitAmount,
            Period = limit.Period,
            CategoryId = limit.CategoryId,
            CategoryName = category?.Name ?? "Desconhecida",
            CategoryIcon = category?.IconName ?? "",
            CurrentSpent = limit.CurrentSpent,
            IsActive = limit.IsActive,
            PeriodStart = limit.PeriodStart,
            PeriodEnd = limit.PeriodEnd,
            CreatedAt = limit.CreatedAt,
            UpdatedAt = limit.UpdatedAt,
            UserId = limit.UserId
        };
    }
}
