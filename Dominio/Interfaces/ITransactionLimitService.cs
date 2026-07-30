using minimal_api.Dominio.DTOs;

namespace minimal_api.Dominio.Interfaces;

/// <summary>
/// Interface para serviço de limites de transações
/// Task 5.6: Implement TransactionLimitService
/// Requirement 19: Notificações de Limite Excedido (Opcional)
/// </summary>
public interface ITransactionLimitService
{
    /// <summary>
    /// Cria um novo limite de transação
    /// Requirement 19.1: permitir que o usuário defina limite
    /// </summary>
    /// <param name="dto">DTO com dados do limite</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>DTO do limite criado</returns>
    Task<TransactionLimitDto> CreateLimitAsync(CreateTransactionLimitDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém um limite específico pelo ID
    /// </summary>
    /// <param name="id">ID do limite</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>DTO do limite ou null se não encontrado</returns>
    Task<TransactionLimitDto?> GetLimitByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista todos os limites com suporte a paginação e filtros
    /// </summary>
    /// <param name="filter">Filtro com página, tamanho da página e filtros opcionais</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resposta paginada com limites</returns>
    Task<PagedTransactionLimitResponseDto> GetLimitsAsync(TransactionLimitFilterDto filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza um limite existente
    /// </summary>
    /// <param name="dto">DTO com dados atualizados do limite</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>DTO do limite atualizado</returns>
    Task<TransactionLimitDto> UpdateLimitAsync(UpdateTransactionLimitDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deleta um limite
    /// </summary>
    /// <param name="id">ID do limite a deletar</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>True se deletado com sucesso, false caso contrário</returns>
    Task<bool> DeleteLimitAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retorna um resumo dos limites e notificações
    /// Requirement 19.5: exibir fila de notificações recentes
    /// </summary>
    /// <param name="userId">ID do usuário (opcional)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resumo de limites e notificações</returns>
    Task<TransactionLimitSummaryDto> GetLimitsSummaryAsync(int? userId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica e atualiza o status de gastos para um limite específico
    /// Requirement 19.2-19.3: verificar se gasto excede 80% e 100%
    /// </summary>
    /// <param name="limitId">ID do limite a verificar</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>DTO do limite atualizado com status de gastos</returns>
    Task<TransactionLimitDto> VerifyAndUpdateLimitStatusAsync(int limitId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Ativa um limite desativado
    /// </summary>
    /// <param name="id">ID do limite a ativar</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>DTO do limite ativado</returns>
    Task<TransactionLimitDto> ActivateLimitAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Desativa um limite
    /// </summary>
    /// <param name="id">ID do limite a desativar</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>DTO do limite desativado</returns>
    Task<TransactionLimitDto> DeactivateLimitAsync(int id, CancellationToken cancellationToken = default);
}
