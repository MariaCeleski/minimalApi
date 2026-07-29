using Microsoft.EntityFrameworkCore;
using minimal_api.Aplicacao.Services;
using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Entidades;
using minimal_api.Infraestrutura.Db;
using minimal_api.Infraestrutura.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace minimal_api.Tests;

/// <summary>
/// Unit tests for Task 2.5: Implement pagination in transaction listing
/// Tests Requirements 2: Listagem de Transações com Paginação
/// </summary>
public class PaginationTests : IDisposable
{
    private readonly DbContexto _context;
    private readonly TransactionRepository _transactionRepository;
    private readonly CategoryRepository _categoryRepository;
    private readonly TransactionService _transactionService;

    public PaginationTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<DbContexto>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new DbContexto(options);
        _transactionRepository = new TransactionRepository(_context);
        _categoryRepository = new CategoryRepository(_context);

        // Setup mocks for validators and logger
        var createValidator = new Mock<IValidator<CreateTransactionDto>>();
        var updateValidator = new Mock<IValidator<UpdateTransactionDto>>();
        var filterValidator = new Mock<IValidator<TransactionFilterDto>>();
        var logger = new Mock<ILogger<TransactionService>>();

        // Setup validator to always pass validation
        createValidator.Setup(v => v.ValidateAsync(It.IsAny<CreateTransactionDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        updateValidator.Setup(v => v.ValidateAsync(It.IsAny<UpdateTransactionDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        filterValidator.Setup(v => v.ValidateAsync(It.IsAny<TransactionFilterDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _transactionService = new TransactionService(
            _transactionRepository,
            _categoryRepository,
            createValidator.Object,
            updateValidator.Object,
            filterValidator.Object,
            logger.Object
        );

        // Seed test data
        SeedTestData().Wait();
    }

    private async Task SeedTestData()
    {
        // Add a test category
        var category = new Category
        {
            Id = 1,
            Name = "Test Category",
            IconName = "test-icon",
            Color = "#FF0000"
        };
        _context.Categories.Add(category);

        // Add 25 test transactions for pagination testing
        for (int i = 1; i <= 25; i++)
        {
            _context.Transactions.Add(new Transaction
            {
                Id = i,
                Amount = 100 + i,
                Date = DateTime.Now.AddDays(-i),
                Type = i % 2 == 0 ? TransactionType.Income : TransactionType.Expense,
                CategoryId = 1,
                Description = $"Test Transaction {i}",
                CreatedAt = DateTime.UtcNow.AddDays(-i),
                UpdatedAt = DateTime.UtcNow.AddDays(-i)
            });
        }

        await _context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetTransactionsAsync_DefaultPagination_ReturnsCorrectMetadata()
    {
        // Arrange
        var filter = new TransactionFilterDto
        {
            Page = 1,
            PageSize = 10
        };

        // Act
        var result = await _transactionService.GetTransactionsAsync(filter);

        // Assert - Task 2.5 requirements
        Assert.Equal(1, result.CurrentPage);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(25, result.TotalItems);
        Assert.Equal(3, result.TotalPages); // Math.Ceiling(25/10) = 3
        Assert.True(result.HasNextPage);
        Assert.False(result.HasPreviousPage);
        Assert.Equal(10, result.Data.Count); // Should return 10 items for first page
    }

    [Fact]
    public async Task GetTransactionsAsync_CustomPageSize_ReturnsCorrectData()
    {
        // Arrange
        var filter = new TransactionFilterDto
        {
            Page = 1,
            PageSize = 5 // Custom page size
        };

        // Act
        var result = await _transactionService.GetTransactionsAsync(filter);

        // Assert - Task 2.5 requirements
        Assert.Equal(1, result.CurrentPage);
        Assert.Equal(5, result.PageSize);
        Assert.Equal(25, result.TotalItems);
        Assert.Equal(5, result.TotalPages); // Math.Ceiling(25/5) = 5
        Assert.True(result.HasNextPage);
        Assert.False(result.HasPreviousPage);
        Assert.Equal(5, result.Data.Count); // Should return 5 items with custom page size
    }

    [Fact]
    public async Task GetTransactionsAsync_SecondPage_ReturnsCorrectMetadata()
    {
        // Arrange
        var filter = new TransactionFilterDto
        {
            Page = 2,
            PageSize = 10
        };

        // Act
        var result = await _transactionService.GetTransactionsAsync(filter);

        // Assert - Task 2.5 requirements
        Assert.Equal(2, result.CurrentPage);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(25, result.TotalItems);
        Assert.Equal(3, result.TotalPages);
        Assert.True(result.HasNextPage);
        Assert.True(result.HasPreviousPage);
        Assert.Equal(10, result.Data.Count); // Should return 10 items for second page
    }

    [Fact]
    public async Task GetTransactionsAsync_LastPage_ReturnsCorrectMetadata()
    {
        // Arrange
        var filter = new TransactionFilterDto
        {
            Page = 3,
            PageSize = 10
        };

        // Act
        var result = await _transactionService.GetTransactionsAsync(filter);

        // Assert - Task 2.5 requirements  
        Assert.Equal(3, result.CurrentPage);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(25, result.TotalItems);
        Assert.Equal(3, result.TotalPages);
        Assert.False(result.HasNextPage);
        Assert.True(result.HasPreviousPage);
        Assert.Equal(5, result.Data.Count); // Last page should have remaining 5 items (25 - 20)
    }

    [Fact]
    public async Task GetTransactionsAsync_EmptyResult_ReturnsCorrectMetadata()
    {
        // Arrange - clear all transactions
        _context.Transactions.RemoveRange(_context.Transactions);
        await _context.SaveChangesAsync();

        var filter = new TransactionFilterDto
        {
            Page = 1,
            PageSize = 10
        };

        // Act
        var result = await _transactionService.GetTransactionsAsync(filter);

        // Assert - Task 2.5 requirements for empty results
        Assert.Equal(1, result.CurrentPage);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(0, result.TotalItems);
        Assert.Equal(0, result.TotalPages);
        Assert.False(result.HasNextPage);
        Assert.False(result.HasPreviousPage);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task GetTransactionsAsync_PageBeyondTotal_ReturnsEmptyData()
    {
        // Arrange
        var filter = new TransactionFilterDto
        {
            Page = 10, // Way beyond available pages
            PageSize = 10
        };

        // Act
        var result = await _transactionService.GetTransactionsAsync(filter);

        // Assert - Task 2.5 requirements for out-of-bounds pages
        Assert.Equal(10, result.CurrentPage);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(25, result.TotalItems);
        Assert.Equal(3, result.TotalPages);
        Assert.False(result.HasNextPage);
        Assert.True(result.HasPreviousPage);
        Assert.Empty(result.Data); // Should return empty data for pages beyond total
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}