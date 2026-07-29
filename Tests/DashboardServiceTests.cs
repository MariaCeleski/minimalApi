using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using minimal_api.Aplicacao.Services;
using minimal_api.Dominio.Interfaces;
using minimal_api.Dominio.Entidades;

namespace minimal_api.Tests;

/// <summary>
/// Testes unitários para o DashboardService
/// Task 3.1: Implement DashboardService with balance calculations
/// Requirements 5, 6: Cálculo Automático de Saldo, Dashboard com Visualização
/// </summary>
public class DashboardServiceTests
{
    private readonly Mock<ITransactionRepository> _mockTransactionRepository;
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly Mock<ILogger<DashboardService>> _mockLogger;
    private readonly DashboardService _dashboardService;

    public DashboardServiceTests()
    {
        _mockTransactionRepository = new Mock<ITransactionRepository>();
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _mockLogger = new Mock<ILogger<DashboardService>>();
        
        _dashboardService = new DashboardService(
            _mockTransactionRepository.Object,
            _mockCategoryRepository.Object,
            _mockLogger.Object);
    }

    /// <summary>
    /// Testa o método GetBalance() com saldo positivo
    /// Requirement 5.1: Saldo = Σ(receitas) - Σ(despesas)
    /// Task 3.1: Criar método GetBalance() retornando saldo total
    /// </summary>
    [Fact]
    public async Task GetBalanceAsync_WithPositiveBalance_ReturnsCorrectBalance()
    {
        // Arrange
        var totalIncome = 1000.50m;
        var totalExpenses = 600.25m;
        var expectedBalance = 400.25m;

        _mockTransactionRepository.Setup(x => x.GetTotalByTypeAsync(TransactionType.Income, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(totalIncome);
        
        _mockTransactionRepository.Setup(x => x.GetTotalByTypeAsync(TransactionType.Expense, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(totalExpenses);

        // Act
        var result = await _dashboardService.GetBalanceAsync();

        // Assert
        Assert.Equal(expectedBalance, result);
        
        // Verify repository calls
        _mockTransactionRepository.Verify(x => x.GetTotalByTypeAsync(TransactionType.Income, null, It.IsAny<CancellationToken>()), Times.Once);
        _mockTransactionRepository.Verify(x => x.GetTotalByTypeAsync(TransactionType.Expense, null, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Testa o método GetBalance() com saldo negativo
    /// Requirement 5.6: marcar saldo devedor
    /// </summary>
    [Fact]
    public async Task GetBalanceAsync_WithNegativeBalance_ReturnsCorrectBalance()
    {
        // Arrange
        var totalIncome = 500.00m;
        var totalExpenses = 750.75m;
        var expectedBalance = -250.75m;

        _mockTransactionRepository.Setup(x => x.GetTotalByTypeAsync(TransactionType.Income, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(totalIncome);
        
        _mockTransactionRepository.Setup(x => x.GetTotalByTypeAsync(TransactionType.Expense, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(totalExpenses);

        // Act
        var result = await _dashboardService.GetBalanceAsync();

        // Assert
        Assert.Equal(expectedBalance, result);
        Assert.True(result < 0, "Balance should be negative");
    }

    /// <summary>
    /// Testa precisão de 2 casas decimais
    /// Requirement 5.5: precisão de 2 casas decimais
    /// Task 3.1: Precisão de 2 casas decimais
    /// </summary>
    [Fact]
    public async Task GetBalanceAsync_WithDecimalPrecision_Returns2DecimalPlaces()
    {
        // Arrange - valores que resultarão em mais de 2 casas decimais
        var totalIncome = 1000.123456m;
        var totalExpenses = 500.654321m;
        var expectedBalance = 499.47m; // Math.Round(499.469135, 2)

        _mockTransactionRepository.Setup(x => x.GetTotalByTypeAsync(TransactionType.Income, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(totalIncome);
        
        _mockTransactionRepository.Setup(x => x.GetTotalByTypeAsync(TransactionType.Expense, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(totalExpenses);

        // Act
        var result = await _dashboardService.GetBalanceAsync();

        // Assert
        Assert.Equal(expectedBalance, result);
        
        // Verificar que tem exatamente 2 casas decimais
        var decimalPlaces = BitConverter.GetBytes(decimal.GetBits(result)[3])[2];
        Assert.True(decimalPlaces <= 2, "Result should have at most 2 decimal places");
    }

    /// <summary>
    /// Testa implementação da fórmula Σ(receitas) - Σ(despesas)
    /// Requirement 5.1: Implementar Σ(receitas) - Σ(despesas)
    /// Task 3.1: Implementar Σ(receitas) - Σ(despesas)
    /// </summary>
    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(1000, 0, 1000)]
    [InlineData(0, 500, -500)]
    [InlineData(1500.50, 750.25, 750.25)]
    [InlineData(100.33, 200.66, -100.33)]
    public async Task GetBalanceAsync_ImplementsCorrectFormula_ReturnsExpectedResult(
        decimal income, decimal expenses, decimal expectedBalance)
    {
        // Arrange
        _mockTransactionRepository.Setup(x => x.GetTotalByTypeAsync(TransactionType.Income, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(income);
        
        _mockTransactionRepository.Setup(x => x.GetTotalByTypeAsync(TransactionType.Expense, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expenses);

        // Act
        var result = await _dashboardService.GetBalanceAsync();

        // Assert
        Assert.Equal(expectedBalance, result);
    }

    /// <summary>
    /// Testa GetTotalsAsync que retorna receitas, despesas e saldo separadamente
    /// Suporte para Requirements 6: Dashboard com visualização detalhada
    /// </summary>
    [Fact]
    public async Task GetTotalsAsync_ReturnsCorrectTotals()
    {
        // Arrange
        var totalIncome = 1200.00m;
        var totalExpenses = 800.50m;
        var expectedBalance = 399.50m;

        _mockTransactionRepository.Setup(x => x.GetTotalByTypeAsync(TransactionType.Income, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(totalIncome);
        
        _mockTransactionRepository.Setup(x => x.GetTotalByTypeAsync(TransactionType.Expense, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(totalExpenses);

        // Act
        var (income, expenses, balance) = await _dashboardService.GetTotalsAsync();

        // Assert
        Assert.Equal(totalIncome, income);
        Assert.Equal(totalExpenses, expenses);
        Assert.Equal(expectedBalance, balance);
        
        // Verificar precisão de 2 casas decimais
        Assert.Equal(Math.Round(totalIncome, 2), income);
        Assert.Equal(Math.Round(totalExpenses, 2), expenses);
        Assert.Equal(Math.Round(expectedBalance, 2), balance);
    }

    /// <summary>
    /// Testa IsBalanceNegativeAsync para identificação de saldo devedor
    /// Requirement 5.6: marcar saldo devedor visualmente
    /// </summary>
    [Theory]
    [InlineData(100, 50, false)]  // Saldo positivo
    [InlineData(50, 100, true)]   // Saldo negativo
    [InlineData(100, 100, false)] // Saldo zero (não negativo)
    [InlineData(0, 0, false)]     // Sem transações
    public async Task IsBalanceNegativeAsync_ReturnsCorrectStatus(
        decimal income, decimal expenses, bool expectedIsNegative)
    {
        // Arrange
        _mockTransactionRepository.Setup(x => x.GetTotalByTypeAsync(TransactionType.Income, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(income);
        
        _mockTransactionRepository.Setup(x => x.GetTotalByTypeAsync(TransactionType.Expense, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expenses);

        // Act
        var result = await _dashboardService.IsBalanceNegativeAsync();

        // Assert
        Assert.Equal(expectedIsNegative, result);
    }

    /// <summary>
    /// Testa que o serviço chama os repositories corretos com parâmetros adequados
    /// Verifica integração entre DashboardService e TransactionRepository
    /// </summary>
    [Fact]
    public async Task GetBalanceAsync_WithUserId_CallsRepositoryWithCorrectParameters()
    {
        // Arrange
        var userId = 123;
        var totalIncome = 500.00m;
        var totalExpenses = 200.00m;

        _mockTransactionRepository.Setup(x => x.GetTotalByTypeAsync(TransactionType.Income, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(totalIncome);
        
        _mockTransactionRepository.Setup(x => x.GetTotalByTypeAsync(TransactionType.Expense, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(totalExpenses);

        // Act
        var result = await _dashboardService.GetBalanceAsync(userId);

        // Assert
        Assert.Equal(300.00m, result);
        
        // Verify que o repository foi chamado com o userId correto
        _mockTransactionRepository.Verify(x => x.GetTotalByTypeAsync(TransactionType.Income, userId, It.IsAny<CancellationToken>()), Times.Once);
        _mockTransactionRepository.Verify(x => x.GetTotalByTypeAsync(TransactionType.Expense, userId, It.IsAny<CancellationToken>()), Times.Once);
    }
}