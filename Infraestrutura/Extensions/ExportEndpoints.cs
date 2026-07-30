using Microsoft.AspNetCore.Mvc;
using minimal_api.Aplicacao.Services;
using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Exceptions;
using minimal_api.Dominio.Interfaces;

namespace minimal_api.Infraestrutura.Extensions;

/// <summary>
/// Extensão para mapeamento de endpoints de exportação
/// Task 4.13: Create GET /export/pdf endpoint
/// Implementa Requirement 12: Exportação de Relatórios em PDF
/// </summary>
public static class ExportEndpoints
{
    /// <summary>
    /// Mapeia todos os endpoints de exportação
    /// </summary>
    public static void MapExportEndpoints(this WebApplication app)
    {
        var exportGroup = app.MapGroup("/api/export")
            .WithTags("Export");

        // GET /api/export/pdf - Exportar relatório para PDF
        // Task 4.13: Create GET /export/pdf endpoint
        // Requirements 12: Exportação em PDF com nome: relatorio_YYYY-MM-DD.pdf
        // Query params: startDate, endDate
        // Retorna arquivo PDF como download
        exportGroup.MapGet("/pdf", async (
            IDashboardService dashboardService,
            ICategoryRepository categoryRepository,
            IExportService exportService,
            CancellationToken cancellationToken,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int? userId = null) =>
        {
            try
            {
                // Validar e preparar datas
                var start = startDate ?? DateTime.UtcNow.AddDays(-30).Date;
                var end = endDate ?? DateTime.UtcNow.Date;

                // Validar período
                if (start > end)
                {
                    return Results.BadRequest(new
                    {
                        error = "Invalid date range",
                        detail = "Start date must be less than or equal to end date",
                        timestamp = DateTime.UtcNow
                    });
                }

                // Obter dados de receitas, despesas e distribuição por categoria
                // Requirement 12: Incluir título, período, resumo e tabela de transações
                var (totalIncome, totalExpenses, balance) = await dashboardService.GetTotalsForPeriodAsync(
                    start,
                    end,
                    userId,
                    cancellationToken);

                // Obter distribuição de despesas por categoria
                var distribution = await dashboardService.GetExpenseDistributionByCategoryAsync(
                    start,
                    end,
                    userId,
                    cancellationToken);

                // Obter informações de categorias
                var categories = await categoryRepository.GetActiveCategoriesAsync();
                var categoryMap = categories.ToDictionary(c => c.Id);

                // Construir lista de categorias para o relatório
                var reportCategories = distribution
                    .Select(kvp =>
                    {
                        var amount = kvp.Value;
                        var percentage = totalExpenses > 0
                            ? Math.Round((amount / totalExpenses) * 100, 2)
                            : 0;

                        var categoryInfo = categoryMap.ContainsKey(kvp.Key)
                            ? categoryMap[kvp.Key]
                            : null;

                        return new MonthlyReportCategoryDto
                        {
                            CategoryId = kvp.Key,
                            CategoryName = categoryInfo?.Name ?? $"Category {kvp.Key}",
                            CategoryIcon = categoryInfo?.IconName ?? string.Empty,
                            CategoryColor = categoryInfo?.Color ?? "#999999",
                            Amount = amount,
                            Percentage = percentage,
                            TransactionCount = 0 // TODO: Adicionar contagem de transações por categoria
                        };
                    })
                    .OrderByDescending(x => x.Amount)
                    .ToList();

                // Criar relatório mensal compatível com ExportService
                var monthlyReport = new MonthlyReportResponseDto
                {
                    Year = start.Year,
                    Month = start.Month,
                    MonthName = start.ToString("MMMM", System.Globalization.CultureInfo.GetCultureInfo("pt-BR")),
                    TotalIncome = totalIncome,
                    TotalExpenses = totalExpenses,
                    TransactionCount = 0, // TODO: Adicionar contagem de transações totais
                    Categories = reportCategories
                };

                // Exportar para PDF usando ExportService
                // Requirement 12.3: Usar formatação visual com cores e fontes legíveis
                var pdfContent = await exportService.ExportReportToPDFAsync(
                    monthlyReport,
                    cancellationToken);

                // Gerar nome do arquivo com data de fim do período
                // Requirement 12.4: Nome: relatorio_YYYY-MM-DD.pdf
                var fileName = $"relatorio_{end:yyyy-MM-dd}.pdf";

                // Retornar como arquivo para download
                // Requirement 12.5: Retornar arquivo como download
                return Results.File(
                    pdfContent,
                    "application/pdf",
                    fileName);
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
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new
                {
                    error = "Invalid operation",
                    detail = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    title: "Failed to export report to PDF",
                    detail: ex.Message,
                    statusCode: 500
                );
            }
        })
        .WithName("ExportReportToPDF")
        .WithSummary("Exportar relatório para PDF")
        .WithDescription("Exporta relatório do período especificado em formato PDF com nome: relatorio_YYYY-MM-DD.pdf. Inclui receitas, despesas, saldo e breakdown por categoria. Parâmetros: startDate (opcional - padrão: 30 dias atrás), endDate (opcional - padrão: hoje), userId (opcional)")
        .Produces<object>(200)
        .Produces<object>(400)
        .Produces<object>(500);
    }
}
