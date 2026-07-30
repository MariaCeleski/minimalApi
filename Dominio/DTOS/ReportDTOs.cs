namespace minimal_api.Dominio.DTOs;

/// <summary>
/// Request DTO para filtro de relatório por categoria
/// Task 4.6: Create GET /reports/category endpoint
/// Requirement 10: Relatório por Categoria
/// </summary>
public class CategoryReportRequestDto
{
    /// <summary>
    /// Data de início do período (opcional)
    /// Se não fornecida, usa 30 dias atrás
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Data de fim do período (opcional)
    /// Se não fornecida, usa data atual
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// ID do usuário (opcional)
    /// Se fornecido, filtra apenas transações do usuário
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// Aplica datas padrão (últimos 30 dias) se não fornecidas
    /// Requirement 3.3-3.4: Use 30 dias atrás e data atual como padrão
    /// </summary>
    public void ApplyDefaults()
    {
        if (!EndDate.HasValue)
        {
            EndDate = DateTime.UtcNow.Date;
        }

        if (!StartDate.HasValue)
        {
            StartDate = EndDate.Value.AddDays(-30);
        }
    }

    /// <summary>
    /// Valida se o período é válido
    /// Requirement 10: Validar que data_inicio <= data_fim
    /// </summary>
    /// <returns>True se válido</returns>
    public bool IsValid()
    {
        return StartDate <= EndDate;
    }
}

/// <summary>
/// Response DTO para relatório mensal
/// Task 4.3: Create GET /reports/monthly endpoint
/// Requirement 9: Relatório Mensal
/// 
/// Inclui:
/// - Receitas totais
/// - Despesas totais
/// - Saldo líquido
/// - Breakdown por categoria com percentuais
/// </summary>
public class MonthlyReportResponseDto
{
    /// <summary>
    /// Ano do relatório
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// Mês do relatório (1-12)
    /// </summary>
    public int Month { get; set; }

    /// <summary>
    /// Nome do mês em português
    /// </summary>
    public string MonthName { get; set; } = string.Empty;

    /// <summary>
    /// Total de receitas no mês
    /// Requirement 9.1: Retornar total de receitas
    /// </summary>
    public decimal TotalIncome { get; set; }

    /// <summary>
    /// Total de despesas no mês
    /// Requirement 9.1: Retornar total de despesas
    /// </summary>
    public decimal TotalExpenses { get; set; }

    /// <summary>
    /// Saldo líquido (receitas - despesas)
    /// Requirement 9.1: Retornar saldo líquido
    /// </summary>
    public decimal Balance => TotalIncome - TotalExpenses;

    /// <summary>
    /// Total de transações no mês
    /// </summary>
    public int TransactionCount { get; set; }

    /// <summary>
    /// Breakdown por categoria
    /// Requirement 9.2: Incluir breakdown por categoria
    /// Requirement 9.3: Incluir percentual de cada categoria em relação ao total
    /// </summary>
    public List<MonthlyReportCategoryDto> Categories { get; set; } = new();
}

/// <summary>
/// Category breakdown para relatório mensal
/// Requirement 9.2-9.5: Informações por categoria
/// </summary>
public class MonthlyReportCategoryDto
{
    /// <summary>
    /// ID da categoria
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// Nome da categoria
    /// </summary>
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// Ícone da categoria (opcional)
    /// </summary>
    public string? CategoryIcon { get; set; }

    /// <summary>
    /// Cor da categoria (opcional)
    /// </summary>
    public string? CategoryColor { get; set; }

    /// <summary>
    /// Valor total de transações nesta categoria no mês
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Percentual desta categoria em relação ao total geral
    /// Requirement 9.3: Incluir percentual de cada categoria
    /// </summary>
    public decimal Percentage { get; set; }

    /// <summary>
    /// Quantidade de transações nesta categoria
    /// </summary>
    public int TransactionCount { get; set; }
}

/// <summary>
/// Response DTO para relatório por categoria
/// Task 4.6: Create GET /reports/category endpoint
/// Requirement 10: Relatório por Categoria
/// 
/// Inclui:
/// - Receitas totais
/// - Despesas totais
/// - Saldo líquido
/// - Agregação por categoria no período especificado
/// </summary>
public class CategoryReportResponseDto
{
    /// <summary>
    /// Etiqueta do período (ex: "01/01/2024 a 31/01/2024")
    /// </summary>
    public string PeriodLabel { get; set; } = string.Empty;

    /// <summary>
    /// Data de início do período
    /// </summary>
    public string StartDate { get; set; } = string.Empty;

    /// <summary>
    /// Data de fim do período
    /// </summary>
    public string EndDate { get; set; } = string.Empty;

    /// <summary>
    /// Total de receitas no período
    /// Requirement 10.1: Retornar total gasto por categoria no período
    /// </summary>
    public decimal TotalIncome { get; set; }

    /// <summary>
    /// Total de despesas no período
    /// Requirement 10.1: Retornar total gasto por categoria no período
    /// </summary>
    public decimal TotalExpenses { get; set; }

    /// <summary>
    /// Saldo líquido (receitas - despesas)
    /// </summary>
    public decimal NetBalance => TotalIncome - TotalExpenses;

    /// <summary>
    /// Total de transações no período
    /// </summary>
    public int TotalTransactionCount { get; set; }

    /// <summary>
    /// Agregação por categoria
    /// Requirement 10.1-10.5: Informações agregadas por categoria
    /// </summary>
    public List<CategoryReportBreakdownDto> Categories { get; set; } = new();
}

/// <summary>
/// Category breakdown para relatório por categoria
/// Requirement 10.1-10.5: Informações por categoria
/// </summary>
public class CategoryReportBreakdownDto
{
    /// <summary>
    /// ID da categoria
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// Nome da categoria
    /// </summary>
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// Ícone da categoria (opcional)
    /// </summary>
    public string? CategoryIcon { get; set; }

    /// <summary>
    /// Cor da categoria (opcional)
    /// </summary>
    public string? CategoryColor { get; set; }

    /// <summary>
    /// Total de receitas nesta categoria no período
    /// Requirement 10.1: Retornar receitas por categoria
    /// </summary>
    public decimal IncomeAmount { get; set; }

    /// <summary>
    /// Total de despesas nesta categoria no período
    /// Requirement 10.1: Retornar despesas por categoria
    /// </summary>
    public decimal ExpenseAmount { get; set; }

    /// <summary>
    /// Saldo líquido nesta categoria (receitas - despesas)
    /// </summary>
    public decimal NetAmount => IncomeAmount - ExpenseAmount;

    /// <summary>
    /// Percentual desta categoria em relação ao total geral
    /// Requirement 10.3: Incluir percentual de cada categoria
    /// </summary>
    public decimal Percentage { get; set; }

    /// <summary>
    /// Quantidade de transações nesta categoria
    /// </summary>
    public int TransactionCount { get; set; }
}
