using Microsoft.AspNetCore.Mvc;
using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Exceptions;
using minimal_api.Dominio.Interfaces;

namespace minimal_api.Infraestrutura.Extensions;

/// <summary>
/// Extensão para mapeamento de endpoints de limites de transações
/// Task 5.8: Create CRUD endpoints para Limits
/// Implementa Requirement 19: Notificações de Limite Excedido
/// </summary>
public static class LimitEndpoints
{
    /// <summary>
    /// Mapeia todos os endpoints de limites de transações
    /// </summary>
    public static void MapLimitEndpoints(this WebApplication app)
    {
        var limitsGroup = app.MapGroup("/api/limits")
            .WithTags("Limits");

        // POST /api/limits - Criar novo limite de transação
        // Requirement 19.1: permitir que o usuário defina limite
        limitsGroup.MapPost("/", async (
            [FromBody] CreateTransactionLimitDto dto,
            ITransactionLimitService limitService,
            CancellationToken cancellationToken) =>
        {
            try
            {
                var result = await limitService.CreateLimitAsync(dto, cancellationToken);
                return Results.Created($"/api/limits/{result.Id}", result);
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
        .WithName("CreateLimit")
        .WithSummary("Criar novo limite de gastos")
        .WithDescription("Cria um novo limite de gastos para uma categoria")
        .Accepts<CreateTransactionLimitDto>("application/json")
        .Produces<TransactionLimitDto>(201)
        .Produces<object>(400)
        .Produces<object>(404);

        // GET /api/limits - Listar limites com paginação
        limitsGroup.MapGet("/", async (
            ITransactionLimitService limitService,
            CancellationToken cancellationToken,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? categoryId = null,
            [FromQuery] int? userId = null,
            [FromQuery] int? period = null,
            [FromQuery] bool? isActive = null) =>
        {
            try
            {
                // Parse period enum se fornecido
                LimitPeriod? limitPeriod = null;
                if (period.HasValue && Enum.IsDefined(typeof(LimitPeriod), period.Value))
                {
                    limitPeriod = (LimitPeriod)period.Value;
                }

                var filter = new TransactionLimitFilterDto
                {
                    Page = page,
                    PageSize = pageSize,
                    CategoryId = categoryId,
                    UserId = userId,
                    Period = limitPeriod,
                    IsActive = isActive
                };

                var result = await limitService.GetLimitsAsync(filter, cancellationToken);
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
        .WithName("GetLimits")
        .WithSummary("Listar limites de gastos")
        .WithDescription("Lista limites com paginação e filtros opcionais")
        .Produces<PagedTransactionLimitResponseDto>(200)
        .Produces<object>(400);

        // GET /api/limits/{id} - Obter limite por ID
        limitsGroup.MapGet("/{id:int}", async (
            [FromRoute] int id,
            ITransactionLimitService limitService,
            CancellationToken cancellationToken) =>
        {
            try
            {
                var result = await limitService.GetLimitByIdAsync(id, cancellationToken);
                
                if (result == null)
                {
                    return Results.NotFound(new
                    {
                        error = $"Limite com ID {id} não encontrado",
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
        .WithName("GetLimitById")
        .WithSummary("Obter limite por ID")
        .WithDescription("Retorna um limite específico pelo seu ID")
        .Produces<TransactionLimitDto>(200)
        .Produces<object>(400)
        .Produces<object>(404);

        // PUT /api/limits/{id} - Atualizar limite
        limitsGroup.MapPut("/{id:int}", async (
            [FromRoute] int id,
            [FromBody] UpdateTransactionLimitDto dto,
            ITransactionLimitService limitService,
            CancellationToken cancellationToken) =>
        {
            try
            {
                // Garantir que o ID do DTO corresponde ao ID da rota
                dto.Id = id;
                
                var result = await limitService.UpdateLimitAsync(dto, cancellationToken);
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
        .WithName("UpdateLimit")
        .WithSummary("Atualizar limite de gastos")
        .WithDescription("Atualiza um limite existente")
        .Accepts<UpdateTransactionLimitDto>("application/json")
        .Produces<TransactionLimitDto>(200)
        .Produces<object>(400)
        .Produces<object>(404);

        // DELETE /api/limits/{id} - Deletar limite
        limitsGroup.MapDelete("/{id:int}", async (
            [FromRoute] int id,
            ITransactionLimitService limitService,
            CancellationToken cancellationToken) =>
        {
            try
            {
                var success = await limitService.DeleteLimitAsync(id, cancellationToken);
                
                if (success)
                {
                    return Results.NoContent();
                }
                else
                {
                    return Results.Problem(
                        title: "Failed to delete limit",
                        detail: $"Unable to delete limit with ID {id}",
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
        .WithName("DeleteLimit")
        .WithSummary("Deletar limite de gastos")
        .WithDescription("Remove um limite do sistema")
        .Produces(204)
        .Produces<object>(400)
        .Produces<object>(404)
        .Produces<object>(500);

        // GET /api/limits/summary - Obter resumo de limites e notificações
        // Requirement 19.5: exibir fila de notificações recentes
        limitsGroup.MapGet("/summary", async (
            ITransactionLimitService limitService,
            CancellationToken cancellationToken,
            [FromQuery] int? userId = null) =>
        {
            try
            {
                var summary = await limitService.GetLimitsSummaryAsync(userId, cancellationToken);
                
                return Results.Ok(summary);
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    title: "Failed to get limits summary",
                    detail: ex.Message,
                    statusCode: 500
                );
            }
        })
        .WithName("GetLimitsSummary")
        .WithSummary("Obter resumo de limites")
        .WithDescription("Retorna um resumo com todas as notificações de limite (warning/alert)")
        .Produces<TransactionLimitSummaryDto>(200)
        .Produces<object>(500);

        // POST /api/limits/{id}/activate - Ativar limite
        limitsGroup.MapPost("/{id:int}/activate", async (
            [FromRoute] int id,
            ITransactionLimitService limitService,
            CancellationToken cancellationToken) =>
        {
            try
            {
                var result = await limitService.ActivateLimitAsync(id, cancellationToken);
                return Results.Ok(result);
            }
            catch (NotFoundException ex)
            {
                return Results.NotFound(new
                {
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
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
        })
        .WithName("ActivateLimit")
        .WithSummary("Ativar limite")
        .WithDescription("Ativa um limite que foi previamente desativado")
        .Produces<TransactionLimitDto>(200)
        .Produces<object>(400)
        .Produces<object>(404);

        // POST /api/limits/{id}/deactivate - Desativar limite
        limitsGroup.MapPost("/{id:int}/deactivate", async (
            [FromRoute] int id,
            ITransactionLimitService limitService,
            CancellationToken cancellationToken) =>
        {
            try
            {
                var result = await limitService.DeactivateLimitAsync(id, cancellationToken);
                return Results.Ok(result);
            }
            catch (NotFoundException ex)
            {
                return Results.NotFound(new
                {
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
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
        })
        .WithName("DeactivateLimit")
        .WithSummary("Desativar limite")
        .WithDescription("Desativa um limite ativo")
        .Produces<TransactionLimitDto>(200)
        .Produces<object>(400)
        .Produces<object>(404);
    }
}
