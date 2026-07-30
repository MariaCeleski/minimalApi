using FluentValidation;
using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Interfaces;
using Microsoft.Extensions.Logging;
using DomainValidationException = minimal_api.Dominio.Exceptions.ValidationException;
using minimal_api.Dominio.Exceptions;
using FluentValidationException = FluentValidation.ValidationException;

namespace minimal_api.Aplicacao.Services;

/// <summary>
/// Serviço para gerenciamento de transações
/// Implementa Requirements 1, 2, 3, 4, 5, 7, 8: CRUD, Filtros, Cálculo de Saldo
/// Task 2.2: TransactionService com validações
/// </summary>
public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IValidator<CreateTransactionDto> _createValidator;
    private readonly IValidator<UpdateTransactionDto> _updateValidator;
    private readonly IValidator<TransactionFilterDto> _filterValidator;
    private readonly ILogger<TransactionService> _logger;

    public TransactionService(
        ITransactionRepository transactionRepository,
        ICategoryRepository categoryRepository,
        IValidator<CreateTransactionDto> createValidator,
        IValidator<UpdateTransactionDto> updateValidator,
        IValidator<TransactionFilterDto> filterValidator,
        ILogger<TransactionService> logger)
    {
        _transactionRepository = transactionRepository;
        _categoryRepository = categoryRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _filterValidator = filterValidator;
        _logger = logger;
    }

    /// <summary>
    /// Cria uma nova transação
    /// Implementa Requirements 1: Cadastro e Validação de Transações
    /// </summary>
    public async Task<TransactionResponseDto> CreateTransactionAsync(CreateTransactionDto dto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Criando nova transação: Valor={Amount}, Tipo={Type}, Categoria={CategoryId}", 
            dto.Amount, dto.Type, dto.CategoryId);

        // Validação com FluentValidation - Requirement 1.1, 1.2, 1.3
        var validationResult = await _createValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validação falhou para nova transação: {Errors}", 
                string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
            throw new DomainValidationException(validationResult.Errors.ToDictionary(
                e => e.PropertyName, 
                e => new[] { e.ErrorMessage }));
        }

        // Verificar se categoria existe - Requirement 1.5
        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId, cancellationToken);
        if (category == null)
        {
            throw new NotFoundException("Category", dto.CategoryId);
        }

        // Criar entidade de transação
        var transaction = new Transaction
        {
            Amount = dto.Amount,
            Date = dto.Date,
            Type = dto.Type,
            CategoryId = dto.CategoryId,
            Description = dto.Description,
            UserId = dto.UserId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Salvar no repositório - Requirement 1.4
        var savedTransaction = await _transactionRepository.AddAsync(transaction, cancellationToken);
        
        _logger.LogInformation("Transação criada com sucesso: ID={Id}", savedTransaction.Id);

        // Retornar DTO de resposta
        return await MapToResponseDto(savedTransaction, category);
    }

    /// <summary>
    /// Obtém uma transação por ID
    /// Implementa Requirements 2: Listagem de Transações (GET by ID)
    /// </summary>
    public async Task<TransactionResponseDto?> GetTransactionByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Buscando transação por ID: {Id}", id);

        if (id <= 0)
        {
            throw new DomainValidationException("ID", new[] { "ID deve ser maior que zero" });
        }

        var transaction = await _transactionRepository.GetByIdAsync(id, cancellationToken);
        if (transaction == null)
        {
            _logger.LogWarning("Transação não encontrada: ID={Id}", id);
            return null;
        }

        return await MapToResponseDto(transaction, transaction.Category);
    }

    /// <summary>
    /// Obtém transações com filtros e paginação
    /// Implementa Requirements 2, 3, 4: Paginação, Filtro por Período, Filtro por Categoria
    /// </summary>
    public async Task<PagedTransactionResponseDto> GetTransactionsAsync(TransactionFilterDto filter, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Buscando transações: Página={Page}, Tamanho={PageSize}, Período={StartDate}-{EndDate}", 
            filter.Page, filter.PageSize, filter.StartDate, filter.EndDate);

        // Validar filtros - Requirements 2, 3, 4
        var validationResult = await _filterValidator.ValidateAsync(filter, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new DomainValidationException(string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        // Aplicar defaults para datas - Requirement 3.3, 3.4
        filter.ApplyDefaults();

        // Buscar transações com filtros
        var pagedResult = await _transactionRepository.GetPagedTransactionsAsync(
            page: filter.Page,
            pageSize: filter.PageSize,
            startDate: filter.StartDate,
            endDate: filter.EndDate,
            categoryIds: filter.CategoryIds,
            type: filter.Type,
            userId: filter.UserId,
            cancellationToken: cancellationToken);

        // Mapear para DTOs de resposta
        var responseDtos = new List<TransactionResponseDto>();
        foreach (var transaction in pagedResult.Data)
        {
            var responseDto = await MapToResponseDto(transaction, transaction.Category);
            responseDtos.Add(responseDto);
        }

        // Calcular resumo - Requirement 5: Cálculo de Saldo
        var summary = await CalculateTransactionSummaryAsync(filter, cancellationToken);

        _logger.LogInformation("Transações encontradas: {Count} de {Total}", 
            responseDtos.Count, pagedResult.TotalItems);

        return new PagedTransactionResponseDto
        {
            Data = responseDtos,
            CurrentPage = pagedResult.CurrentPage,
            PageSize = pagedResult.PageSize,
            TotalItems = pagedResult.TotalItems,
            TotalPages = pagedResult.TotalPages,
            Summary = summary
        };
    }

    /// <summary>
    /// Atualiza uma transação existente
    /// Implementa Requirements 7: Edição de Transações
    /// </summary>
    public async Task<TransactionResponseDto> UpdateTransactionAsync(UpdateTransactionDto dto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Atualizando transação: ID={Id}, Valor={Amount}, Tipo={Type}", 
            dto.Id, dto.Amount, dto.Type);

        // Validação com FluentValidation - Requirement 7.2
        var validationResult = await _updateValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validação falhou para atualização da transação {Id}: {Errors}", 
                dto.Id, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
            throw new DomainValidationException(string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        // Verificar se transação existe - Requirement 7.4
        var existingTransaction = await _transactionRepository.GetByIdAsync(dto.Id, cancellationToken);
        if (existingTransaction == null)
        {
            throw new NotFoundException("Transaction", dto.Id);
        }

        // Verificar se categoria existe - Requirement 7.2
        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId, cancellationToken);
        if (category == null)
        {
            throw new NotFoundException("Category", dto.CategoryId);
        }

        // Atualizar campos - Requirement 7.1: ID e CreatedAt não mudam
        existingTransaction.Amount = dto.Amount;
        existingTransaction.Date = dto.Date;
        existingTransaction.Type = dto.Type;
        existingTransaction.CategoryId = dto.CategoryId;
        existingTransaction.Description = dto.Description;
        existingTransaction.UpdatedAt = DateTime.UtcNow;
        // ID e CreatedAt permanecem inalterados

        // Salvar alterações - Requirement 7.5: recalcular saldo automaticamente
        var updatedTransaction = await _transactionRepository.UpdateAsync(existingTransaction, cancellationToken);

        _logger.LogInformation("Transação atualizada com sucesso: ID={Id}", dto.Id);

        return await MapToResponseDto(updatedTransaction, category);
    }

    /// <summary>
    /// Remove uma transação
    /// Implementa Requirements 8: Exclusão de Transações
    /// </summary>
    public async Task<bool> DeleteTransactionAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Removendo transação: ID={Id}", id);

        if (id <= 0)
        {
            throw new DomainValidationException("ID deve ser maior que zero");
        }

        // Verificar se transação existe - Requirement 8.1
        var existingTransaction = await _transactionRepository.GetByIdAsync(id, cancellationToken);
        if (existingTransaction == null)
        {
            _logger.LogWarning("Tentativa de excluir transação inexistente: ID={Id}", id);
            throw new NotFoundException("Transaction", id);
        }

        // Remover transação - Requirement 8.2, 8.3: recalcular saldo automaticamente
        var deleted = await _transactionRepository.DeleteAsync(id, cancellationToken);

        if (deleted)
        {
            _logger.LogInformation("Transação removida com sucesso: ID={Id}", id);
        }
        else
        {
            _logger.LogError("Falha ao remover transação: ID={Id}", id);
        }

        return deleted;
    }

    /// <summary>
    /// Calcula o saldo total do usuário
    /// Implementa Requirements 5: Cálculo Automático de Saldo
    /// </summary>
    public async Task<decimal> CalculateBalanceAsync(int? userId = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Calculando saldo total para usuário: {UserId}", userId?.ToString() ?? "Todos");

        // Requirement 5.1: Saldo = Σ(receitas) - Σ(despesas)
        var totalIncome = await _transactionRepository.GetTotalByTypeAsync(TransactionType.Income, userId, cancellationToken);
        var totalExpenses = await _transactionRepository.GetTotalByTypeAsync(TransactionType.Expense, userId, cancellationToken);

        var balance = totalIncome - totalExpenses;

        _logger.LogInformation("Saldo calculado: Receitas={Income}, Despesas={Expenses}, Saldo={Balance}", 
            totalIncome, totalExpenses, balance);

        return Math.Round(balance, 2); // Requirement 5.5: precisão de 2 casas decimais
    }

    /// <summary>
    /// Calcula resumo de transações para um período
    /// Suporte para dashboard e relatórios
    /// </summary>
    private async Task<TransactionSummaryDto> CalculateTransactionSummaryAsync(TransactionFilterDto filter, CancellationToken cancellationToken)
    {
        var totalIncome = await _transactionRepository.GetTotalByTypeInPeriodAsync(
            TransactionType.Income, filter.StartDate!.Value, filter.EndDate!.Value, filter.UserId, cancellationToken);
        
        var totalExpenses = await _transactionRepository.GetTotalByTypeInPeriodAsync(
            TransactionType.Expense, filter.StartDate!.Value, filter.EndDate!.Value, filter.UserId, cancellationToken);

        var transactionCount = await _transactionRepository.GetCountInPeriodAsync(
            filter.StartDate!.Value, filter.EndDate!.Value, filter.UserId, cancellationToken);

        return new TransactionSummaryDto
        {
            TotalIncome = Math.Round(totalIncome, 2),
            TotalExpenses = Math.Round(totalExpenses, 2),
            TransactionCount = transactionCount,
            StartDate = filter.StartDate!.Value,
            EndDate = filter.EndDate!.Value
        };
    }

    /// <summary>
    /// Mapeia Transaction para TransactionResponseDto
    /// Inclui informações da categoria - Requirement 16: ícones por categoria
    /// </summary>
    private async Task<TransactionResponseDto> MapToResponseDto(Transaction transaction, Category? category = null)
    {
        // Buscar categoria se não fornecida
        category ??= await _categoryRepository.GetByIdAsync(transaction.CategoryId);

        return new TransactionResponseDto
        {
            Id = transaction.Id,
            Amount = transaction.Amount,
            Date = transaction.Date,
            Type = transaction.Type,
            CategoryId = transaction.CategoryId,
            CategoryName = category?.Name ?? "Categoria não encontrada",
            CategoryIcon = category?.IconName ?? "help-circle",
            CategoryColor = category?.Color ?? "#A4B0BE",
            Description = transaction.Description,
            CreatedAt = transaction.CreatedAt,
            UpdatedAt = transaction.UpdatedAt,
            UserId = transaction.UserId
        };
    }
}

/// <summary>
/// Interface para o TransactionService
/// Define contrato público do serviço
/// </summary>
public interface ITransactionService
{
    /// <summary>
    /// Cria uma nova transação
    /// </summary>
    Task<TransactionResponseDto> CreateTransactionAsync(CreateTransactionDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém uma transação por ID
    /// </summary>
    Task<TransactionResponseDto?> GetTransactionByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém transações com filtros e paginação
    /// </summary>
    Task<PagedTransactionResponseDto> GetTransactionsAsync(TransactionFilterDto filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza uma transação existente
    /// </summary>
    Task<TransactionResponseDto> UpdateTransactionAsync(UpdateTransactionDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove uma transação
    /// </summary>
    Task<bool> DeleteTransactionAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Calcula o saldo total do usuário
    /// </summary>
    Task<decimal> CalculateBalanceAsync(int? userId = null, CancellationToken cancellationToken = default);
}