namespace minimal_api.Dominio.Interfaces;

/// <summary>
/// Interface para serviço de exportação de dados
/// Task 4.8: ExportService with CSV export
/// Task 4.11: ExportService with PDF export
/// Requirements 11, 12: Exportação de Transações em CSV e PDF
/// </summary>
public interface IExportService
{
    /// <summary>
    /// Exporta transações para CSV
    /// Task 4.8: Implement ExportService with CSV export
    /// Requirement 11: Exportação em CSV com headers: ID, Data, Tipo, Valor, Categoria, Descrição
    /// </summary>
    /// <param name="transactions">Lista de transações para exportar</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Conteúdo CSV em formato string</returns>
    Task<string> ExportTransactionsToCSVAsync(
        IEnumerable<global::minimal_api.Dominio.DTOs.TransactionResponseDto> transactions,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Exporta relatório para PDF
    /// Task 4.11: Implement ExportService with PDF export
    /// Requirement 12: Exportação em PDF com título, período, resumo, tabela de transações e cores/formatação
    /// </summary>
    /// <param name="report">Relatório mensal para exportar</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Conteúdo PDF em formato byte array</returns>
    Task<byte[]> ExportReportToPDFAsync(
        global::minimal_api.Dominio.DTOs.MonthlyReportResponseDto report,
        CancellationToken cancellationToken = default);
}
