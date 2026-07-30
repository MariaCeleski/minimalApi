using System.ComponentModel.DataAnnotations;
using minimal_api.Dominio.Entidades;

namespace minimal_api.Dominio.DTOs;

/// <summary>
/// DTO para criação de nova transação
/// Implementa Requirements 1: Cadastro e Validação de Transações
/// Valida: data, valor, categoria, descrição obrigatórios
/// </summary>
public class CreateTransactionDto
{
    /// <summary>
    /// Valor da transação - deve ser maior que zero
    /// Requirement 1.2: rejeitar transações com valor <= 0
    /// </summary>
    [Required(ErrorMessage = "O valor é obrigatório")]
    [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]
    public decimal Amount { get; set; }

    /// <summary>
    /// Data da transação - não pode ser futura
    /// Requirement 1.3: validar se data não ultrapassa data atual
    /// </summary>
    [Required(ErrorMessage = "A data é obrigatória")]
    public DateTime Date { get; set; }

    /// <summary>
    /// Tipo da transação (Receita ou Despesa)
    /// Requirement 1: diferenciação entre receitas e despesas
    /// </summary>
    [Required(ErrorMessage = "O tipo da transação é obrigatório")]
    [EnumDataType(typeof(TransactionType), ErrorMessage = "Tipo de transação inválido")]
    public TransactionType Type { get; set; }

    /// <summary>
    /// ID da categoria - deve existir no sistema
    /// Requirement 1.5: suportar 8 categorias predefinidas
    /// </summary>
    [Required(ErrorMessage = "A categoria é obrigatória")]
    [Range(1, int.MaxValue, ErrorMessage = "ID da categoria deve ser válido")]
    public int CategoryId { get; set; }

    /// <summary>
    /// Descrição da transação - obrigatória, até 255 caracteres
    /// Requirement 1.1: campos obrigatórios incluem descrição
    /// Requirement 1.6: permitir strings até 255 caracteres
    /// </summary>
    [Required(ErrorMessage = "A descrição é obrigatória")]
    [StringLength(255, MinimumLength = 3, ErrorMessage = "A descrição deve ter entre 3 e 255 caracteres")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// ID do usuário (opcional para versão sem autenticação)
    /// Para futuro uso com Requirements 17: Autenticação
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// Validação customizada para data não futura
    /// Implementa Requirement 1.3: data não pode ultrapassar data atual
    /// </summary>
    public bool IsDateValid()
    {
        return Date <= DateTime.Now.Date.AddDays(1); // Permite até final do dia atual
    }
}

/// <summary>
/// DTO para atualização de transação existente
/// Implementa Requirements 7: Edição de Transações
/// Mantém mesmas validações do CreateTransactionDto
/// </summary>
public class UpdateTransactionDto
{
    /// <summary>
    /// ID da transação a ser atualizada
    /// Requirement 7.4: validar que ID da transação existe
    /// </summary>
    [Required(ErrorMessage = "O ID da transação é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "ID da transação deve ser válido")]
    public int Id { get; set; }

    /// <summary>
    /// Valor da transação - deve ser maior que zero
    /// Requirement 7.2: validar mesmas regras do cadastro
    /// </summary>
    [Required(ErrorMessage = "O valor é obrigatório")]
    [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]
    public decimal Amount { get; set; }

    /// <summary>
    /// Data da transação - não pode ser futura
    /// Requirement 7.2: data não futura durante edição
    /// </summary>
    [Required(ErrorMessage = "A data é obrigatória")]
    public DateTime Date { get; set; }

    /// <summary>
    /// Tipo da transação (Receita ou Despesa)
    /// </summary>
    [Required(ErrorMessage = "O tipo da transação é obrigatório")]
    [EnumDataType(typeof(TransactionType), ErrorMessage = "Tipo de transação inválido")]
    public TransactionType Type { get; set; }

    /// <summary>
    /// ID da categoria - deve existir no sistema
    /// </summary>
    [Required(ErrorMessage = "A categoria é obrigatória")]
    [Range(1, int.MaxValue, ErrorMessage = "ID da categoria deve ser válido")]
    public int CategoryId { get; set; }

    /// <summary>
    /// Descrição da transação - obrigatória, até 255 caracteres
    /// </summary>
    [Required(ErrorMessage = "A descrição é obrigatória")]
    [StringLength(255, MinimumLength = 3, ErrorMessage = "A descrição deve ter entre 3 e 255 caracteres")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Validação customizada para data não futura
    /// </summary>
    public bool IsDateValid()
    {
        return Date <= DateTime.Now.Date.AddDays(1);
    }
}

/// <summary>
/// DTO de resposta para transações
/// Implementa Requirements 2: Listagem de Transações
/// Retorna campos: ID, data, valor, categoria, descrição, tipo
/// </summary>
public class TransactionResponseDto
{
    /// <summary>
    /// ID único da transação
    /// Requirement 2.5: retornar ID em cada transação
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Valor da transação formatado
    /// Requirement 5.5: precisão de 2 casas decimais
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Data da transação
    /// Requirement 2.5: retornar data
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Tipo da transação (Receita/Despesa)
    /// Requirement 2.5: retornar tipo
    /// </summary>
    public TransactionType Type { get; set; }

    /// <summary>
    /// Nome do tipo para exibição
    /// Facilita uso no frontend
    /// </summary>
    public string TypeName => Type == TransactionType.Income ? "Receita" : "Despesa";

    /// <summary>
    /// ID da categoria
    /// Requirement 2.5: retornar categoria
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// Nome da categoria para exibição
    /// Evita necessidade de lookup no frontend
    /// </summary>
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// Ícone da categoria para exibição
    /// Requirement 16: ícones por categoria
    /// </summary>
    public string CategoryIcon { get; set; } = string.Empty;

    /// <summary>
    /// Cor da categoria para exibição
    /// Suporte para UI colorida
    /// </summary>
    public string CategoryColor { get; set; } = string.Empty;

    /// <summary>
    /// Descrição da transação
    /// Requirement 2.5: retornar descrição
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Data de criação do registro
    /// Para auditoria e ordenação
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
/// DTO para filtros de listagem de transações
/// Implementa Requirements 2, 3, 4: Paginação, Filtro por Período, Filtro por Categoria
/// </summary>
public class TransactionFilterDto
{
    /// <summary>
    /// Página atual para paginação
    /// Requirement 2.2: página N retorna itens (N-1)*10 até N*10
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "A página deve ser maior que zero")]
    public int Page { get; set; } = 1;

    /// <summary>
    /// Tamanho da página
    /// Requirement 2.1: tamanho padrão de 10 itens
    /// </summary>
    [Range(1, 100, ErrorMessage = "O tamanho da página deve estar entre 1 e 100")]
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Data de início do filtro (opcional)
    /// Requirement 3.3: quando omitida, usar 30 dias atrás
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Data de fim do filtro (opcional)
    /// Requirement 3.4: quando omitida, usar data atual
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Lista de IDs de categorias para filtro
    /// Requirement 4.2: permitir múltiplas categorias
    /// </summary>
    public List<int> CategoryIds { get; set; } = new List<int>();

    /// <summary>
    /// Tipo de transação para filtro (opcional)
    /// Permite filtrar apenas receitas ou apenas despesas
    /// </summary>
    public TransactionType? Type { get; set; }

    /// <summary>
    /// ID do usuário para filtro (quando autenticação implementada)
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// Aplica valores padrão para datas quando não fornecidas
    /// Requirement 3.3 e 3.4: defaults de 30 dias atrás e data atual
    /// </summary>
    public void ApplyDefaults()
    {
        StartDate ??= DateTime.Now.AddDays(-30).Date;
        EndDate ??= DateTime.Now.Date.AddDays(1).AddTicks(-1); // Final do dia atual
    }

    /// <summary>
    /// Valida se o filtro de período é válido
    /// Requirement 3.2: validar que data_inicio <= data_fim
    /// </summary>
    public bool IsDateRangeValid()
    {
        if (!StartDate.HasValue || !EndDate.HasValue)
            return true; // Será aplicado defaults

        return StartDate <= EndDate;
    }
}

/// <summary>
/// DTO para resposta paginada de transações
/// Implementa Requirements 2: metadados de paginação
/// </summary>
public class PagedTransactionResponseDto
{
    /// <summary>
    /// Lista de transações da página atual
    /// </summary>
    public List<TransactionResponseDto> Data { get; set; } = new List<TransactionResponseDto>();

    /// <summary>
    /// Página atual
    /// Requirement 2.3: incluir página atual nos metadados
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Tamanho da página
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total de itens encontrados
    /// Requirement 2.3: incluir total de itens nos metadados
    /// </summary>
    public int TotalItems { get; set; }

    /// <summary>
    /// Total de páginas
    /// Requirement 2.3: incluir total de páginas nos metadados
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Indica se há próxima página
    /// Facilita navegação no frontend
    /// </summary>
    public bool HasNextPage => CurrentPage < TotalPages;

    /// <summary>
    /// Indica se há página anterior
    /// Facilita navegação no frontend
    /// </summary>
    public bool HasPreviousPage => CurrentPage > 1;

    /// <summary>
    /// Resumo dos filtros aplicados
    /// Para exibição no frontend
    /// </summary>
    public TransactionSummaryDto? Summary { get; set; }
}

/// <summary>
/// DTO para resumo de transações
/// Suporte para dashboard e relatórios
/// </summary>
public class TransactionSummaryDto
{
    /// <summary>
    /// Total de receitas no período filtrado
    /// </summary>
    public decimal TotalIncome { get; set; }

    /// <summary>
    /// Total de despesas no período filtrado
    /// </summary>
    public decimal TotalExpenses { get; set; }

    /// <summary>
    /// Saldo (receitas - despesas)
    /// Requirement 5.1: cálculo automático de saldo
    /// </summary>
    public decimal Balance => TotalIncome - TotalExpenses;

    /// <summary>
    /// Indica se o saldo é negativo
    /// Requirement 5.6: marcar saldo devedor
    /// </summary>
    public bool IsNegative => Balance < 0;

    /// <summary>
    /// Quantidade de transações no período
    /// </summary>
    public int TransactionCount { get; set; }

    /// <summary>
    /// Período do resumo
    /// </summary>
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}