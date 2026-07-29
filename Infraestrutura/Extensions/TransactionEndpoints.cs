using Microsoft.AspNetCore.Mvc;
using minimal_api.Aplicacao.Services;
using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Exceptions;
using minimal_api.Dominio.Entidades;

namespace minimal_api.Infraestrutura.Extensions;

/// <summary>
/// Extensão para mapeamento de endpoints de transações
/// Task 2.4: Create Transaction API endpoints (POST, GET, GET by ID)
/// Implementa Requirements 1, 2: Cadastro e Listagem de Transações
/// </summary>
public static class TransactionEndpoints
{
    /// <summary>
    /// Mapeia todos os endpoints de transações
    /// </summary>
    public static void MapTransactionEndpoints(this WebApplication app)
    {
        var transactionGroup = app.MapGroup("/api/transactions")
            .WithTags("Transactions");

        // POST /api/transactions - Criar transação
        // Requirement 1: Cadastro e Validação de Transações
        transactionGroup.MapPost("/", async (
            [FromBody] CreateTransactionDto dto,
            ITransactionService transactionService,
            CancellationToken cancellationToken) =>
        {
            try
            {
                var result = await transactionService.CreateTransactionAsync(dto, cancellationToken);
                return Results.Created($"/api/transactions/{result.Id}", result);
            }
            catch (ValidationException ex)
            {
                return Results.BadRequest(new
                {
                    error = "Validation failed",
                    details = ex.Errors,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (NotFoundException ex)
            {
                return Results.NotFound(new
                {
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        })
        .WithName("CreateTransaction")
        .WithSummary("Criar nova transação")
        .WithDescription("Cria uma nova transação de receita ou despesa com validações")
        .Accepts<CreateTransactionDto>("application/json")
        .Produces<TransactionResponseDto>(201)
        .Produces<object>(400)
        .Produces<object>(404);

        // GET /api/transactions - Listar transações com paginação
        // Requirements 2, 3, 4: Listagem, Filtro por Período, Filtro por Categoria
        transactionGroup.MapGet("/", async (
            ITransactionService transactionService,
            CancellationToken cancellationToken,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? categoryIds = null,
            [FromQuery] int? type = null,
            [FromQuery] int? userId = null) =>
        {
            try
            {
                // Parse categoryIds se fornecido
                var categoryIdList = new List<int>();
                if (!string.IsNullOrEmpty(categoryIds))
                {
                    var categoryIdStrings = categoryIds.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var categoryIdString in categoryIdStrings)
                    {
                        if (int.TryParse(categoryIdString.Trim(), out var categoryId))
                        {
                            categoryIdList.Add(categoryId);
                        }
                    }
                }

                // Parse type enum se fornecido
                TransactionType? transactionType = null;
                if (type.HasValue && Enum.IsDefined(typeof(TransactionType), type.Value))
                {
                    transactionType = (TransactionType)type.Value;
                }

                var filter = new TransactionFilterDto
                {
                    Page = page,
                    PageSize = pageSize,
                    StartDate = startDate,
                    EndDate = endDate,
                    CategoryIds = categoryIdList,
                    Type = transactionType,
                    UserId = userId
                };

                var result = await transactionService.GetTransactionsAsync(filter, cancellationToken);
                return Results.Ok(result);
            }
            catch (ValidationException ex)
            {
                return Results.BadRequest(new
                {
                    error = "Invalid filter parameters",
                    details = ex.Errors,
                    timestamp = DateTime.UtcNow
                });
            }
        })
        .WithName("GetTransactions")
        .WithSummary("Listar transações")
        .WithDescription("Lista transações com paginação e filtros opcionais por período e categoria")
        .Produces<PagedTransactionResponseDto>(200)
        .Produces<object>(400);

        // GET /api/transactions/{id} - Obter transação por ID
        // Requirement 2: Listagem de Transações (GET by ID)
        transactionGroup.MapGet("/{id:int}", async (
            [FromRoute] int id,
            ITransactionService transactionService,
            CancellationToken cancellationToken) =>
        {
            try
            {
                var result = await transactionService.GetTransactionByIdAsync(id, cancellationToken);
                
                if (result == null)
                {
                    return Results.NotFound(new
                    {
                        error = $"Transação com ID {id} não encontrada",
                        timestamp = DateTime.UtcNow
                    });
                }

                return Results.Ok(result);
            }
            catch (ValidationException ex)
            {
                return Results.BadRequest(new
                {
                    error = "Invalid ID format",
                    details = ex.Errors,
                    timestamp = DateTime.UtcNow
                });
            }
        })
        .WithName("GetTransactionById")
        .WithSummary("Obter transação por ID")
        .WithDescription("Retorna uma transação específica pelo seu ID")
        .Produces<TransactionResponseDto>(200)
        .Produces<object>(400)
        .Produces<object>(404);

        // PUT /api/transactions/{id} - Atualizar transação
        // Requirement 7: Edição de Transações
        transactionGroup.MapPut("/{id:int}", async (
            [FromRoute] int id,
            [FromBody] UpdateTransactionDto dto,
            ITransactionService transactionService,
            CancellationToken cancellationToken) =>
        {
            try
            {
                // Garantir que o ID do DTO corresponde ao ID da rota
                dto.Id = id;
                
                var result = await transactionService.UpdateTransactionAsync(dto, cancellationToken);
                return Results.Ok(result);
            }
            catch (ValidationException ex)
            {
                return Results.BadRequest(new
                {
                    error = "Validation failed",
                    details = ex.Errors,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (NotFoundException ex)
            {
                return Results.NotFound(new
                {
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        })
        .WithName("UpdateTransaction")
        .WithSummary("Atualizar transação")
        .WithDescription("Atualiza uma transação existente com validações")
        .Accepts<UpdateTransactionDto>("application/json")
        .Produces<TransactionResponseDto>(200)
        .Produces<object>(400)
        .Produces<object>(404);

        // DELETE /api/transactions/{id} - Deletar transação
        // Requirement 8: Exclusão de Transações
        transactionGroup.MapDelete("/{id:int}", async (
            [FromRoute] int id,
            ITransactionService transactionService,
            CancellationToken cancellationToken) =>
        {
            try
            {
                var success = await transactionService.DeleteTransactionAsync(id, cancellationToken);
                
                if (success)
                {
                    return Results.NoContent();
                }
                else
                {
                    return Results.Problem(
                        title: "Failed to delete transaction",
                        detail: $"Unable to delete transaction with ID {id}",
                        statusCode: 500
                    );
                }
            }
            catch (ValidationException ex)
            {
                return Results.BadRequest(new
                {
                    error = "Invalid ID format",
                    details = ex.Errors,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (NotFoundException ex)
            {
                return Results.NotFound(new
                {
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        })
        .WithName("DeleteTransaction")
        .WithSummary("Deletar transação")
        .WithDescription("Remove uma transação do sistema")
        .Produces(204)
        .Produces<object>(400)
        .Produces<object>(404)
        .Produces<object>(500);

        // GET /api/transactions/balance - Calcular saldo
        // Requirement 5: Cálculo Automático de Saldo
        transactionGroup.MapGet("/balance", async (
            ITransactionService transactionService,
            CancellationToken cancellationToken,
            [FromQuery] int? userId = null) =>
        {
            try
            {
                var balance = await transactionService.CalculateBalanceAsync(userId, cancellationToken);
                
                return Results.Ok(new
                {
                    balance = balance,
                    isNegative = balance < 0,
                    timestamp = DateTime.UtcNow,
                    userId = userId
                });
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    title: "Failed to calculate balance",
                    detail: ex.Message,
                    statusCode: 500
                );
            }
        })
        .WithName("GetBalance")
        .WithSummary("Calcular saldo atual")
        .WithDescription("Calcula o saldo atual (receitas - despesas)")
        .Produces<object>(200)
        .Produces<object>(500);
    }
}