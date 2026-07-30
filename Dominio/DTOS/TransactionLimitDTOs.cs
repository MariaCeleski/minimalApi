using System.ComponentModel.DataAnnotations;
using minimal_api.Dominio.Entidades;

namespace minimal_api.Dominio.DTOs;

/// <summary>
/// DTO para criação de novo limite de transação
/// Implementa Requirement 19: Notificações de Limite Excedido
/// Permite que o usuário defina limite de gastos por categoria
/// </summary>
public class CreateTransactionLimitDto
{
    /// <summary>
    /// Nome do limite
    /// Requirement 19.1: permitir que o usuário defina limite
    /// </summary>
    [Required(ErrorMessage = "Nome do limite é obrigatório")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Nome deve ter entre 3 e 200 caracteres")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Valor do limite de gastos
    /// Requirement 19: limite de gastos por categoria
    /// Deve ser maior que zero
    /// </summary>
    [Required(ErrorMessage = "Valor do limite é obrigatório")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Valor do limite deve ser maior que zero")]
    public decimal LimitAmount { get; set; }

    /// <summary>
    /// Período do limite (Diário, Semanal, Mensal, Anual)
    /// Define o intervalo para reset automático e cálculo de gastos
    /// </summary>
    [Required(ErrorMessage = "Período do limite é obrigatório")]
    [EnumDataType(typeof(LimitPeriod), ErrorMessage = "Período inválido")]
    public LimitPeriod Period { get; set; }

    /// <summary>
    /// ID da categoria para aplicar o limite
    /// Requirement 19.1: limite de gastos por categoria
    /// </summary>
    [Required(ErrorMessage = "Categoria é obrigatória")]
    [Range(1, int.MaxValue, ErrorMessage = "Categoria deve ser selecionada")]
    public int CategoryId { get; set; }

    /// <summary>
    /// ID do usuário (opcional para limites globais)
    /// Quando não fornecido, o limite é aplicado globalmente
    /// </summary>
    public int? UserId { get; set; }
}

/// <summary>
/// DTO para atualização de limite de transação
/// Implementa Requirement 19: Edição de limites
/// Mantém mesmas validações do CreateTransactionLimitDto
/// </summary>
public class UpdateTransactionLimitDto
{
    /// <summary>
    /// ID do limite a ser atualizado
    /// </summary>
    [Required(ErrorMessage = "ID do limite é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "ID do limite deve ser válido")]
    public int Id { get; set; }

    /// <summary>
    /// Nome do limite
    /// </summary>
    [Required(ErrorMessage = "Nome do limite é obrigatório")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Nome deve ter entre 3 e 200 caracteres")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Valor do limite de gastos
    /// </summary>
    [Required(ErrorMessage = "Valor do limite é obrigatório")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Valor do limite deve ser maior que zero")]
    public decimal LimitAmount { get; set; }

    /// <summary>
    /// Período do limite
    /// </summary>
    [Required(ErrorMessage = "Período do limite é obrigatório")]
    [EnumDataType(typeof(LimitPeriod), ErrorMessage = "Período inválido")]
    public LimitPeriod Period { get; set; }

    /// <summary>
    /// ID da categoria
    /// </summary>
    [Required(ErrorMessage = "Categoria é obrigatória")]
    [Range(1, int.MaxValue, ErrorMessage = "Categoria deve ser selecionada")]
    public int CategoryId { get; set; }

    /// <summary>
    /// Status de ativação do limite
    /// </summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// DTO de resposta para limite de transação
/// Implementa Requirement 19: Retorna informações de limite com status
/// </summary>
public class TransactionLimitDto
{
    /// <summary>
    /// ID único do limite
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nome do limite
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Valor do limite de gastos
    /// Requirement 19: limite de gastos por categoria
    /// </summary>
    public decimal LimitAmount { get; set; }

    /// <summary>
    /// Período do limite
    /// </summary>
    public LimitPeriod Period { get; set; }

    /// <summary>
    /// Descrição do período para exibição
    /// </summary>
    public string PeriodName => Period switch
    {
        LimitPeriod.Daily => "Diário",
        LimitPeriod.Weekly => "Semanal",
        LimitPeriod.Monthly => "Mensal",
        LimitPeriod.Yearly => "Anual",
        _ => "Indefinido"
    };

    /// <summary>
    /// ID da categoria
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// Nome da categoria para exibição
    /// Evita lookup no frontend
    /// </summary>
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// Ícone da categoria para exibição
    /// </summary>
    public string CategoryIcon { get; set; } = string.Empty;

    /// <summary>
    /// Gasto atual no período
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal CurrentSpent { get; set; }

    /// <summary>
    /// Valor restante até atingir o limite
    /// Requirement 19: para monitoramento visual
    /// </summary>
    public decimal RemainingAmount => Math.Max(LimitAmount - CurrentSpent, 0);

    /// <summary>
    /// Percentual de uso do limite
    /// Requirement 19.2-19.3: threshold de 80% e 100%
    /// </summary>
    public decimal UsagePercentage => LimitAmount > 0 ? (CurrentSpent / LimitAmount) * 100 : 0;

    /// <summary>
    /// Indica se o limite foi excedido
    /// Requirement 19.3: quando 100%+
    /// </summary>
    public bool IsExceeded => CurrentSpent > LimitAmount;

    /// <summary>
    /// Indica se está próximo do limite (80%+)
    /// Requirement 19.2: aviso a 80%+
    /// </summary>
    public bool IsNearLimit => UsagePercentage >= 80;

    /// <summary>
    /// Indica se está na zona de aviso (80%-99.99%)
    /// </summary>
    public bool IsWarningZone => UsagePercentage >= 80 && UsagePercentage < 100;

    /// <summary>
    /// Status de notificação
    /// "warning" para 80%+, "alert" para 100%+, null caso contrário
    /// </summary>
    public string? NotificationStatus
    {
        get
        {
            if (IsExceeded)
                return "alert";
            if (IsNearLimit)
                return "warning";
            return null;
        }
    }

    /// <summary>
    /// Se o limite está ativo
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Data do início do período atual
    /// </summary>
    public DateTime PeriodStart { get; set; }

    /// <summary>
    /// Data do fim do período atual
    /// </summary>
    public DateTime PeriodEnd { get; set; }

    /// <summary>
    /// Data de criação do registro
    /// Para auditoria
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Data de última atualização
    /// Para auditoria
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// ID do usuário (quando autenticação estiver implementada)
    /// </summary>
    public int? UserId { get; set; }
}

/// <summary>
/// DTO para resposta paginada de limites de transação
/// Suporte para listagem de limites com metadados de paginação
/// </summary>
public class PagedTransactionLimitResponseDto
{
    /// <summary>
    /// Lista de limites da página atual
    /// </summary>
    public List<TransactionLimitDto> Data { get; set; } = new List<TransactionLimitDto>();

    /// <summary>
    /// Página atual
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Tamanho da página
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total de itens encontrados
    /// </summary>
    public int TotalItems { get; set; }

    /// <summary>
    /// Total de páginas
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Indica se há próxima página
    /// </summary>
    public bool HasNextPage => CurrentPage < TotalPages;

    /// <summary>
    /// Indica se há página anterior
    /// </summary>
    public bool HasPreviousPage => CurrentPage > 1;
}

/// <summary>
/// DTO para filtro de listagem de limites
/// Suporte para buscar limites com filtros específicos
/// </summary>
public class TransactionLimitFilterDto
{
    /// <summary>
    /// Página atual para paginação
    /// </summary>
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    /// <summary>
    /// Tamanho da página
    /// </summary>
    [Range(1, 100)]
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// ID do usuário para filtro
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// ID da categoria para filtro
    /// </summary>
    public int? CategoryId { get; set; }

    /// <summary>
    /// Período para filtro (opcional)
    /// </summary>
    public LimitPeriod? Period { get; set; }

    /// <summary>
    /// Filtrar apenas limites ativos
    /// </summary>
    public bool? IsActive { get; set; }
}

/// <summary>
/// DTO para resumo de limites e notificações
/// Requirement 19.5: exibir fila de notificações recentes
/// </summary>
public class TransactionLimitSummaryDto
{
    /// <summary>
    /// Total de limites definidos
    /// </summary>
    public int TotalLimits { get; set; }

    /// <summary>
    /// Quantidade de limites ativos
    /// </summary>
    public int ActiveLimits { get; set; }

    /// <summary>
    /// Quantidade de limites em zona de aviso (80%+)
    /// </summary>
    public int WarningCount { get; set; }

    /// <summary>
    /// Quantidade de limites excedidos (100%+)
    /// </summary>
    public int ExceededCount { get; set; }

    /// <summary>
    /// Limites que estão em zona de aviso
    /// Para exibição no dashboard
    /// </summary>
    public List<TransactionLimitDto> WarningLimits { get; set; } = new List<TransactionLimitDto>();

    /// <summary>
    /// Limites que foram excedidos
    /// Para exibição como alertas
    /// </summary>
    public List<TransactionLimitDto> ExceededLimits { get; set; } = new List<TransactionLimitDto>();

    /// <summary>
    /// Gasto total do período atual em todas as categorias
    /// </summary>
    public decimal TotalSpent { get; set; }

    /// <summary>
    /// Limite total de todas as categorias
    /// </summary>
    public decimal TotalLimit { get; set; }

    /// <summary>
    /// Percentual de gasto total
    /// </summary>
    public decimal TotalUsagePercentage => TotalLimit > 0 ? (TotalSpent / TotalLimit) * 100 : 0;
}
