using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using minimal_api.Aplicacao.Services;
using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Entidades;
using minimal_api.Infraestrutura.Db;
using minimal_api.Infraestrutura.Repositories;
using minimal_api.Dominio.Validators;
using FluentValidation;
using Xunit;

namespace minimal_api.Tests;

/// <summary>
/// Testes para verificar a funcionalidade de paginação nas transações
/// Task 2.5: Implement pagination in transaction listing
/// Requirements 2: Listagem com Paginação
/// </summary>
public class PaginationTests : IDisposable
{
    private readonly DbContexto _context;
    private readonly TransactionService _transactionService;
    private readonly TransactionRepository _transactionRepository;
    private readonly CategoryRepository _categoryRepository;
    
    public PaginationTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<DbContexto>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new DbContexto(options);
        _transactionRepository = new TransactionRepository(_context);
        _categoryRepository = new CategoryRepository(_context);
        
        // Setup logger
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger<TransactionService>();
        
        // Setup validators - usando versões simples para testes unitários
        var createValidator = new SimpleCreateTransactionValidator();
        var updateValidator = new SimpleUpdateTransactionValidator();
        var filterValidator = new SimpleTransactionFilterValidator();
        
        _transactionService = new TransactionService(
            _transactionRepository,
            _categoryRepository,
            createValidator,
            updateValidator,
            filterValidator,
            logger);
        
        // Initialize database with seed data
        SeedDatabase();
    }
    
    private void SeedDatabase()
    {
        // Add categories first
        var categories = new[]
        {
            new Category { Id = 1, Name = "Alimentação", IconName = "utensils", Color = "#FF6B6B" },
            new Category { Id = 2, Name = "Transporte", IconName = "car", Color = "#4ECDC4" },
            new Category { Id = 3, Name = "Lazer", IconName = "smile", Color = "#45B7D1" }
        };
        
        _context.Categories.AddRange(categories);
        _context.SaveChanges();
        
        // Add transactions for pagination testing
        var transactions = new List<Transaction>();
        for (int i = 1; i <= 25; i++)
        {
            transactions.Add(new Transaction
            {
                Id = i,
                Amount = i * 10,
                Date = DateTime.Now.AddDays(-i),
                Type = i % 2 == 0 ? TransactionType.Income : TransactionType.Expense,
                CategoryId = ((i - 1) % 3) + 1,
                Description = $"Transação teste {i}",
                UserId = 1,
                CreatedAt = DateTime.UtcNow.AddDays(-i),
                UpdatedAt = DateTime.UtcNow.AddDays(-i)
            });
        }
        
        _context.Transactions.AddRange(transactions);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetTransactions_WithDefaultPagination_ShouldReturn10Items()
    {
        // Arrange
        var filter = new TransactionFilterDto
        {
            Page = 1,
            PageSize = 10
        };

        // Act
        var result = await _transactionService.GetTransactionsAsync(filter);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Data.Count);
        Assert.Equal(1, result.CurrentPage);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(25, result.TotalItems);
        Assert.Equal(3, result.TotalPages); // 25 items / 10 per page = 3 pages
        Assert.False(result.HasPreviousPage);
        Assert.True(result.HasNextPage);
    }

    [Fact]
    public async Task GetTransactions_WithPage2_ShouldReturn10Items()
    {
        // Arrange
        var filter = new TransactionFilterDto
        {
            Page = 2,
            PageSize = 10
        };

        // Act
        var result = await _transactionService.GetTransactionsAsync(filter);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Data.Count);
        Assert.Equal(2, result.CurrentPage);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(25, result.TotalItems);
        Assert.Equal(3, result.TotalPages);
        Assert.True(result.HasPreviousPage);
        Assert.True(result.HasNextPage);
    }

    [Fact]
    public async Task GetTransactions_WithLastPage_ShouldReturn5Items()
    {
        // Arrange
        var filter = new TransactionFilterDto
        {
            Page = 3,
            PageSize = 10
        };

        // Act
        var result = await _transactionService.GetTransactionsAsync(filter);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.Data.Count); // Last page has only 5 items (25 - 20)
        Assert.Equal(3, result.CurrentPage);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(25, result.TotalItems);
        Assert.Equal(3, result.TotalPages);
        Assert.True(result.HasPreviousPage);
        Assert.False(result.HasNextPage);
    }

    [Fact]
    public async Task GetTransactions_WithCustomPageSize_ShouldReturnCorrectItems()
    {
        // Arrange
        var filter = new TransactionFilterDto
        {
            Page = 1,
            PageSize = 5
        };

        // Act
        var result = await _transactionService.GetTransactionsAsync(filter);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.Data.Count);
        Assert.Equal(1, result.CurrentPage);
        Assert.Equal(5, result.PageSize);
        Assert.Equal(25, result.TotalItems);
        Assert.Equal(5, result.TotalPages); // 25 items / 5 per page = 5 pages
        Assert.False(result.HasPreviousPage);
        Assert.True(result.HasNextPage);
    }

    [Fact]
    public async Task GetTransactions_WithInvalidPage_ShouldReturnPage1()
    {
        // Arrange
        var filter = new TransactionFilterDto
        {
            Page = 0, // Invalid page
            PageSize = 10
        };

        // Act & Assert - Should validate and return page 1 or throw validation error
        var exception = await Assert.ThrowsAsync<minimal_api.Dominio.Exceptions.ValidationException>(
            () => _transactionService.GetTransactionsAsync(filter));
        
        Assert.NotNull(exception);
    }

    [Fact]
    public async Task GetTransactions_WithPageBeyondTotal_ShouldReturnEmptyPage()
    {
        // Arrange
        var filter = new TransactionFilterDto
        {
            Page = 10, // Page beyond total pages
            PageSize = 10
        };

        // Act
        var result = await _transactionService.GetTransactionsAsync(filter);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Data);
        Assert.Equal(10, result.CurrentPage);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(25, result.TotalItems);
        Assert.Equal(3, result.TotalPages);
        Assert.True(result.HasPreviousPage);
        Assert.False(result.HasNextPage);
    }

    [Fact]
    public async Task GetTransactions_ShouldReturnCorrectMetadata()
    {
        // Arrange
        var filter = new TransactionFilterDto
        {
            Page = 2,
            PageSize = 8
        };

        // Act
        var result = await _transactionService.GetTransactionsAsync(filter);

        // Assert - Check pagination metadata (Requirement 2.3)
        Assert.NotNull(result);
        Assert.Equal(2, result.CurrentPage);
        Assert.Equal(8, result.PageSize);
        Assert.Equal(25, result.TotalItems);
        Assert.Equal(4, result.TotalPages); // ceiling(25/8) = 4
        Assert.True(result.HasPreviousPage);
        Assert.True(result.HasNextPage);
    }

    [Fact]
    public async Task GetTransactions_ShouldReturnOrderedByDateDescending()
    {
        // Arrange
        var filter = new TransactionFilterDto
        {
            Page = 1,
            PageSize = 5
        };

        // Act
        var result = await _transactionService.GetTransactionsAsync(filter);

        // Assert - Requirement 2.2: ordem decrescente por data
        Assert.NotNull(result);
        Assert.Equal(5, result.Data.Count);
        
        for (int i = 0; i < result.Data.Count - 1; i++)
        {
            Assert.True(result.Data[i].Date >= result.Data[i + 1].Date,
                $"Transaction {i} date {result.Data[i].Date} should be >= transaction {i+1} date {result.Data[i + 1].Date}");
        }
    }

    [Fact]
    public async Task GetTransactions_WithSummary_ShouldReturnTransactionSummary()
    {
        // Arrange
        var filter = new TransactionFilterDto
        {
            Page = 1,
            PageSize = 10
        };

        // Act
        var result = await _transactionService.GetTransactionsAsync(filter);

        // Assert - Should include summary for dashboard support
        Assert.NotNull(result);
        Assert.NotNull(result.Summary);
        Assert.True(result.Summary.TotalIncome > 0 || result.Summary.TotalExpenses > 0);
        Assert.Equal(25, result.Summary.TransactionCount);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}