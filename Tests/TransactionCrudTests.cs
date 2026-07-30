using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.DTOs;
using minimal_api.Infraestrutura.Db;
using minimal_api.Infraestrutura.Repositories;
using minimal_api.Aplicacao.Services;
using minimal_api.Dominio.Validators;
using minimal_api.Dominio.Exceptions;
using Xunit;

namespace minimal_api.Tests;

/// <summary>
/// Comprehensive tests for Transaction CRUD operations
/// Task 2.15: Ensure all transaction CRUD tests pass
/// Validates Requirements 1, 2, 3, 4, 5, 7, 8
/// </summary>
public class TransactionCrudTests : IDisposable
{
    private readonly DbContexto _context;
    private readonly TransactionService _transactionService;
    private readonly List<Category> _testCategories;

    public TransactionCrudTests()
    {
        var options = new DbContextOptionsBuilder<DbContexto>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new DbContexto(options);

        // Seed test categories
        _testCategories = new List<Category>
        {
            new() { Id = 1, Name = "Alimentação", IconName = "utensils", Color = "#FF6B6B", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = 2, Name = "Transporte", IconName = "car", Color = "#4ECDC4", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = 3, Name = "Lazer", IconName = "coffee", Color = "#45B7D1", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = 4, Name = "Saúde", IconName = "heart", Color = "#96CEB4", IsActive = true, CreatedAt = DateTime.UtcNow }
        };

        _context.Categories.AddRange(_testCategories);
        _context.SaveChanges();

        // Setup service dependencies
        var transactionRepository = new TransactionRepository(_context);
        var categoryRepository = new CategoryRepository(_context);
        var createValidator = new CreateTransactionDtoValidator(categoryRepository);
        var updateValidator = new UpdateTransactionDtoValidator(categoryRepository);
        var filterValidator = new TransactionFilterDtoValidator(categoryRepository);
        var logger = new LoggerFactory().CreateLogger<TransactionService>();

        _transactionService = new TransactionService(
            transactionRepository,
            categoryRepository,
            createValidator,
            updateValidator,
            filterValidator,
            logger);
    }

    #region Create Transaction Tests (Requirement 1)

    [Fact]
    public async Task CreateTransaction_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var createDto = new CreateTransactionDto
        {
            Amount = 100.50m,
            Date = DateTime.Now.Date,
            Type = TransactionType.Expense,
            CategoryId = 1,
            Description = "Test transaction",
            UserId = null
        };

        // Act
        var result = await _transactionService.CreateTransactionAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(createDto.Amount, result.Amount);
        Assert.Equal(createDto.Type, result.Type);
        Assert.Equal("Alimentação", result.CategoryName);
    }

    [Fact]
    public async Task CreateTransaction_WithZeroAmount_ShouldThrowException()
    {
        // Arrange
        var createDto = new CreateTransactionDto
        {
            Amount = 0,
            Date = DateTime.Now.Date,
            Type = TransactionType.Expense,
            CategoryId = 1,
            Description = "Invalid transaction"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => 
            _transactionService.CreateTransactionAsync(createDto));
    }

    [Fact]
    public async Task CreateTransaction_WithInvalidCategory_ShouldThrowValidationException()
    {
        // Arrange
        var createDto = new CreateTransactionDto
        {
            Amount = 100,
            Date = DateTime.Now.Date,
            Type = TransactionType.Expense,
            CategoryId = 999,
            Description = "Invalid category transaction"
        };

        // Act & Assert - FluentValidation catches this before service logic
        await Assert.ThrowsAsync<ValidationException>(() => 
            _transactionService.CreateTransactionAsync(createDto));
    }

    #endregion

    #region Read Transaction Tests (Requirement 2)

    [Fact]
    public async Task GetTransactionById_WithExistingId_ShouldReturnTransaction()
    {
        // Arrange
        var transaction = await CreateTestTransaction(100, TransactionType.Income, 1);

        // Act
        var result = await _transactionService.GetTransactionByIdAsync(transaction.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(transaction.Id, result.Id);
        Assert.Equal(transaction.Amount, result.Amount);
    }

    [Fact]
    public async Task GetTransactionById_WithNonExistingId_ShouldReturnNull()
    {
        // Act
        var result = await _transactionService.GetTransactionByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetTransactions_WithPagination_ShouldReturnPagedResults()
    {
        // Arrange
        await CreateMultipleTestTransactions();
        var filter = new TransactionFilterDto { Page = 1, PageSize = 5 };

        // Act
        var result = await _transactionService.GetTransactionsAsync(filter);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Data.Count <= 5);
        Assert.Equal(1, result.CurrentPage);
        Assert.True(result.TotalItems > 0);
    }

    #endregion

    #region Update Transaction Tests (Requirement 7)

    [Fact]
    public async Task UpdateTransaction_WithValidData_ShouldUpdateSuccessfully()
    {
        // Arrange
        var original = await CreateTestTransaction(100, TransactionType.Expense, 1);
        
        var updateDto = new UpdateTransactionDto
        {
            Id = original.Id,
            Amount = 150.75m,
            Date = DateTime.Now.Date.AddDays(-1),
            Type = TransactionType.Income,
            CategoryId = 2,
            Description = "Updated transaction"
        };

        // Act
        var result = await _transactionService.UpdateTransactionAsync(updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(original.Id, result.Id);
        Assert.Equal(updateDto.Amount, result.Amount);
        Assert.Equal(updateDto.Type, result.Type);
        Assert.Equal("Transporte", result.CategoryName);
    }

    [Fact]
    public async Task UpdateTransaction_WithNonExistingId_ShouldThrowNotFoundException()
    {
        // Arrange
        var updateDto = new UpdateTransactionDto
        {
            Id = 999,
            Amount = 100,
            Date = DateTime.Now.Date,
            Type = TransactionType.Expense,
            CategoryId = 1,
            Description = "Non-existing transaction"
        };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => 
            _transactionService.UpdateTransactionAsync(updateDto));
    }

    #endregion

    #region Delete Transaction Tests (Requirement 8)

    [Fact]
    public async Task DeleteTransaction_WithExistingId_ShouldDeleteSuccessfully()
    {
        // Arrange
        var transaction = await CreateTestTransaction(100, TransactionType.Expense, 1);
        var originalBalance = await _transactionService.CalculateBalanceAsync();

        // Act
        var result = await _transactionService.DeleteTransactionAsync(transaction.Id);

        // Assert
        Assert.True(result);
        
        // Verify transaction is deleted
        var deletedTransaction = await _transactionService.GetTransactionByIdAsync(transaction.Id);
        Assert.Null(deletedTransaction);
        
        // Verify balance is recalculated
        var newBalance = await _transactionService.CalculateBalanceAsync();
        Assert.Equal(originalBalance + 100, newBalance); // Expense was removed, so balance increases
    }

    [Fact]
    public async Task DeleteTransaction_WithNonExistingId_ShouldThrowNotFoundException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => 
            _transactionService.DeleteTransactionAsync(999));
    }

    #endregion

    #region Balance Calculation Tests (Requirement 5)

    [Fact]
    public async Task CalculateBalance_WithMixedTransactions_ShouldCalculateCorrectly()
    {
        // Arrange
        await CreateTestTransaction(1000, TransactionType.Income, 1);   // +1000
        await CreateTestTransaction(200, TransactionType.Expense, 2);   // -200
        await CreateTestTransaction(500, TransactionType.Income, 3);    // +500
        await CreateTestTransaction(150, TransactionType.Expense, 4);   // -150

        // Expected balance: 1000 - 200 + 500 - 150 = 1150

        // Act
        var balance = await _transactionService.CalculateBalanceAsync();

        // Assert
        Assert.Equal(1150, balance);
    }

    [Fact]
    public async Task CalculateBalance_WithNoTransactions_ShouldReturnZero()
    {
        // Act
        var balance = await _transactionService.CalculateBalanceAsync();

        // Assert
        Assert.Equal(0, balance);
    }

    #endregion

    #region Filtering Tests (Requirements 3, 4)

    [Fact]
    public async Task GetTransactions_WithCategoryFilter_ShouldReturnFilteredResults()
    {
        // Arrange
        var transaction1 = await CreateTestTransaction(100, TransactionType.Income, 1);
        var transaction2 = await CreateTestTransaction(200, TransactionType.Expense, 2);
        var transaction3 = await CreateTestTransaction(300, TransactionType.Income, 3);

        var filter = new TransactionFilterDto 
        { 
            CategoryIds = new List<int> { 1, 3 },
            Page = 1, 
            PageSize = 10 
        };

        // Act
        var result = await _transactionService.GetTransactionsAsync(filter);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Data.Any(t => t.Id == transaction1.Id));
        Assert.False(result.Data.Any(t => t.Id == transaction2.Id));
        Assert.True(result.Data.Any(t => t.Id == transaction3.Id));
    }

    [Fact]
    public async Task GetTransactions_WithPeriodFilter_ShouldReturnFilteredResults()
    {
        // Arrange
        var today = DateTime.Now.Date;
        var todayTransaction = await CreateTestTransactionWithDate(100, TransactionType.Income, 1, today);
        var yesterdayTransaction = await CreateTestTransactionWithDate(200, TransactionType.Expense, 2, today.AddDays(-1));

        var filter = new TransactionFilterDto 
        { 
            StartDate = today,
            EndDate = today.AddDays(1),
            Page = 1, 
            PageSize = 10 
        };

        // Act
        var result = await _transactionService.GetTransactionsAsync(filter);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Data.Any(t => t.Id == todayTransaction.Id));
        Assert.False(result.Data.Any(t => t.Id == yesterdayTransaction.Id));
    }

    #endregion

    #region Data Integrity Tests (Requirement 20)

    [Fact]
    public async Task TransactionOperations_ShouldMaintainDataIntegrity()
    {
        // Arrange - Create transactions and calculate initial balance
        await CreateTestTransaction(1000, TransactionType.Income, 1);
        await CreateTestTransaction(300, TransactionType.Expense, 2);
        var initialBalance = await _transactionService.CalculateBalanceAsync();

        // Act - Perform multiple operations
        var newTransaction = await CreateTestTransaction(200, TransactionType.Income, 3);
        var balanceAfterAdd = await _transactionService.CalculateBalanceAsync();
        
        await _transactionService.DeleteTransactionAsync(newTransaction.Id);
        var balanceAfterDelete = await _transactionService.CalculateBalanceAsync();

        // Assert - Balance should be consistent
        Assert.Equal(initialBalance + 200, balanceAfterAdd);
        Assert.Equal(initialBalance, balanceAfterDelete);
    }

    #endregion

    #region Helper Methods

    private async Task<TransactionResponseDto> CreateTestTransaction(decimal amount, TransactionType type, int categoryId)
    {
        var createDto = new CreateTransactionDto
        {
            Amount = amount,
            Date = DateTime.Now.Date,
            Type = type,
            CategoryId = categoryId,
            Description = $"Test {type} transaction - {amount:C}",
            UserId = null
        };

        return await _transactionService.CreateTransactionAsync(createDto);
    }

    private async Task<TransactionResponseDto> CreateTestTransactionWithDate(decimal amount, TransactionType type, int categoryId, DateTime date)
    {
        var createDto = new CreateTransactionDto
        {
            Amount = amount,
            Date = date,
            Type = type,
            CategoryId = categoryId,
            Description = $"Test {type} transaction - {amount:C} on {date:yyyy-MM-dd}",
            UserId = null
        };

        return await _transactionService.CreateTransactionAsync(createDto);
    }

    private async Task CreateMultipleTestTransactions()
    {
        var transactions = new[]
        {
            (500m, TransactionType.Income, 1, "Salary"),
            (50m, TransactionType.Expense, 2, "Transport"),
            (30m, TransactionType.Expense, 1, "Lunch"),
            (200m, TransactionType.Income, 3, "Freelance"),
            (80m, TransactionType.Expense, 4, "Medicine"),
            (25m, TransactionType.Expense, 3, "Coffee"),
            (1000m, TransactionType.Income, 1, "Bonus"),
            (40m, TransactionType.Expense, 2, "Gas"),
            (15m, TransactionType.Expense, 1, "Snack"),
            (300m, TransactionType.Income, 4, "Consultation")
        };

        foreach (var (amount, type, categoryId, description) in transactions)
        {
            var createDto = new CreateTransactionDto
            {
                Amount = amount,
                Date = DateTime.Now.Date.AddDays(Random.Shared.Next(-10, 1)),
                Type = type,
                CategoryId = categoryId,
                Description = description,
                UserId = null
            };

            await _transactionService.CreateTransactionAsync(createDto);
        }
    }

    #endregion

    public void Dispose()
    {
        _context?.Dispose();
    }
}