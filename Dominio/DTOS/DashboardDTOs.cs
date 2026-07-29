using System.ComponentModel.DataAnnotations;

namespace minimal_api.Dominio.DTOs;

/// <summary>
/// DTO para resposta do dashboard principal
/// Implementa Requirements 6: Dashboard com Visualização de Saldo e Gráficos
/// Task 3.1: Implement DashboardService with balance calculations
/// </summary>
public class DashboardResponseDto
{
    /// <summary>
    /// Saldo total atual do usuário
    /// Requirement 5.1: Saldo = Σ(receitas) - Σ(despesas)
    /// Requirement 5.5: precisão de 2 casas decimais
    /// </summary>
    public decimal Balance { get; set; }

    /// <summary>
    /// Total de receitas
    /// Para exibição detalhada no dashboard
    /// </summary>
    public decimal TotalIncome { get; set; }

    /// <summary>
    /// Total de despesas
    /// Para exibição detalhada no dashboard
    /// </summary>
    public decimal TotalExpenses { get; set; }

    /// <summary>
    /// Indica se o saldo é negativo (devedor)
    /// Requirement 5.6: marcar saldo devedor visualmente
    /// Requirement 6.7: indicador visual em cor de alerta se negativo
    /// </summary>
    public bool IsNegative => Balance < 0;

    /// <summary>
    /// Última atualização dos dados
    /// Para controle de cache e tempo real
    /// </summary>
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Período dos dados (quando aplicado filtro)
    /// Suporte para Requirement 6.6: filtro de período no dashboard
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Final do período dos dados
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Número total de transações no período
    /// Para estatísticas gerais
    /// </summary>
    public int TransactionCount { get; set; }
}

/// <summary>
/// DTO para distribuição de despesas por categoria
/// Implementa Requirements 6.2: gráfico de pizza com distribuição por categoria
/// </summary>
public class CategoryDistributionResponseDto
{
    /// <summary>
    /// Lista de categorias com valores
    /// Requirement 6.2: distribuição de despesas por categoria
    /// </summary>
    public List<CategoryDistributionItemDto> Categories { get; set; } = new List<CategoryDistributionItemDto>();

    /// <summary>
    /// Total geral de despesas
    /// Para validação e porcentagens
    /// </summary>
    public decimal TotalExpenses { get; set; }

    /// <summary>
    /// Período da distribuição
    /// </summary>
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Última atualização dos dados
    /// </summary>
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Item da distribuição de categoria
/// </summary>
public class CategoryDistributionItemDto
{
    /// <summary>
    /// ID da categoria
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// Nome da categoria para exibição
    /// </summary>
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// Ícone da categoria
    /// Requirement 16: ícones por categoria
    /// </summary>
    public string CategoryIcon { get; set; } = string.Empty;

    /// <summary>
    /// Cor da categoria para o gráfico
    /// </summary>
    public string CategoryColor { get; set; } = string.Empty;

    /// <summary>
    /// Valor total gasto na categoria
    /// Requirement 5.5: precisão de 2 casas decimais
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Percentual em relação ao total
    /// Para exibição no gráfico de pizza
    /// </summary>
    public decimal Percentage { get; set; }
}

/// <summary>
/// DTO para tendência mensal do saldo
/// Implementa Requirements 6.3: gráfico de linha com evolução dos últimos 12 meses
/// </summary>
public class MonthlyTrendResponseDto
{
    /// <summary>
    /// Lista de pontos da tendência mensal
    /// Requirement 6.3: evolução do saldo ao longo dos meses
    /// </summary>
    public List<MonthlyTrendItemDto> TrendData { get; set; } = new List<MonthlyTrendItemDto>();

    /// <summary>
    /// Número de meses incluídos
    /// </summary>
    public int MonthsCount { get; set; }

    /// <summary>
    /// Última atualização dos dados
    /// </summary>
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Item da tendência mensal
/// </summary>
public class MonthlyTrendItemDto
{
    /// <summary>
    /// Mês de referência (primeiro dia do mês)
    /// </summary>
    public DateTime Month { get; set; }

    /// <summary>
    /// Nome do mês para exibição (ex: "Jan 2024")
    /// </summary>
    public string MonthName => Month.ToString("MMM yyyy");

    /// <summary>
    /// Saldo acumulado até o final do mês
    /// Requirement 5.5: precisão de 2 casas decimais
    /// </summary>
    public decimal Balance { get; set; }

    /// <summary>
    /// Total de receitas no mês
    /// </summary>
    public decimal Income { get; set; }

    /// <summary>
    /// Total de despesas no mês
    /// </summary>
    public decimal Expenses { get; set; }

    /// <summary>
    /// Resultado do mês (receitas - despesas)
    /// </summary>
    public decimal NetResult => Income - Expenses;
}

/// <summary>
/// DTO para filtros do dashboard
/// Suporte para Requirements 6.6: aplicar filtros no dashboard
/// </summary>
public class DashboardFilterDto
{
    /// <summary>
    /// Data de início do período (opcional)
    /// Quando não informada, usa padrões do sistema
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Data de fim do período (opcional)
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// ID do usuário (para futura implementação de autenticação)
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// Aplica valores padrão quando datas não são fornecidas
    /// </summary>
    public void ApplyDefaults()
    {
        StartDate ??= DateTime.Now.AddDays(-30).Date;
        EndDate ??= DateTime.Now.Date.AddDays(1).AddTicks(-1);
    }

    /// <summary>
    /// Valida se o período é válido
    /// </summary>
    public bool IsValid()
    {
        if (!StartDate.HasValue || !EndDate.HasValue)
            return true;
            
        return StartDate <= EndDate;
    }
}

/// <summary>
/// DTO para resposta completa do dashboard
/// Combina todas as informações em uma única resposta
/// Implementa Requirements 6: dashboard completo com saldo, gráficos e filtros
/// </summary>
public class CompleteDashboardResponseDto
{
    /// <summary>
    /// Informações de saldo e totais
    /// </summary>
    public DashboardResponseDto Summary { get; set; } = new DashboardResponseDto();

    /// <summary>
    /// Distribuição de despesas por categoria
    /// Requirement 6.2: gráfico de pizza
    /// </summary>
    public CategoryDistributionResponseDto CategoryDistribution { get; set; } = new CategoryDistributionResponseDto();

    /// <summary>
    /// Tendência mensal do saldo
    /// Requirement 6.3: gráfico de linha dos últimos 12 meses
    /// </summary>
    public MonthlyTrendResponseDto MonthlyTrend { get; set; } = new MonthlyTrendResponseDto();

    /// <summary>
    /// Filtros aplicados
    /// Para referência do frontend
    /// </summary>
    public DashboardFilterDto AppliedFilters { get; set; } = new DashboardFilterDto();

    /// <summary>
    /// Timestamp da geração dos dados
    /// Para controle de cache
    /// </summary>
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}