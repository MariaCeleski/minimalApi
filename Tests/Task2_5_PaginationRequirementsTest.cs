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
/// Testes específicos para Task 2.5: Implement pagination in transaction listing
/// Verifica os requisitos exatos da tarefa:
/// - Adicionar parameters: page, pageSize (default 10)
/// - Retornar metadados: currentPage, totalPages, totalItems
/// - Requirements: 2
/// </summary>
public class Task2_5_PaginationRequirementsTest : IDisposable
{
    private readonly DbContexto _context;
    private readonly TransactionService _transactionService;
    private readonly TransactionRepository _transactionRepository;
    private readonly CategoryRepository _categoryRepository;
    
    public Task2_5_PaginationRequirementsTest()
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
        
        // Setup validators
        var createValidator = new CreateTransactionDtoValidator(_categoryRepository);
        var updateValidator = new UpdateTransactionDtoValidator(_categoryRepository);
        var filterValidator = new TransactionFilterDtoValidator(_categoryRepository);
        
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
        // Add categories
        var categories = new[]
        {
            new Category { Id = 1, Name = "Alimentação", IconName = "utensils", Color = "#FF6B6B" },
            new Category { Id = 2, Name = "Transporte", IconName = "car", Color = "#4ECDC4" }
        };
        
        _context.Categories.AddRange(categories);
        _context.SaveChanges();
        
        // Add 15 transactions to test pagination with default pageSize 10
        var transactions = new List<Transaction>();
        for (int i = 1; i <= 15; i++)
        {
            transactions.Add(new Transaction
            {
                Id = i,
                Amount = i * 10,
                Date = DateTime.Now.AddDays(-i),
                Type = i % 2 == 0 ? TransactionType.Income : TransactionType.Expense,
                CategoryId = ((i - 1) % 2) + 1,
                Description = $"Transaction {i}",
                UserId = 1,
                CreatedAt = DateTime.UtcNow.AddDays(-i),
                UpdatedAt = DateTime.UtcNow.AddDays(-i)
            });
        }
        
        _context.Transactions.AddRange(transactions);
        _context.SaveChanges();
    }

    [Fact]
    public async Task Task2_5_DefaultPageSize_ShouldBe10()
    {
        // Arrange - Task 2.5 requirement: default pageSize = 10
        var filter = new TransactionFilterDto
        {
            Page = 1
            // PageSize not specified, should default to 10
        };

        // Act
        var result = await _transactionService.GetTransactionsAsync(filter);

        // Assert - Verify default pageSize is 10
        Assert.NotNull(result);
        Assert.Equal(10, result.PageSize); // Should default to 10
        Assert.Equal(10, result.Data.Count); // Should return 10 items (we have 15 total)
        
        Console.WriteLine($"✓ Default pageSize confirmed as {result.PageSize}");
    }

    [Fact]
    public async Task Task2_5_PageParameter_ShouldWork()
    {
        // Arrange - Task 2.5 requirement: page parameter
        var filterPage1 = new TransactionFilterDto { Page = 1, PageSize = 10 };
        var filterPage2 = new TransactionFilterDto { Page = 2, PageSize = 10 };

        // Act
        var resultPage1 = await _transactionService.GetTransactionsAsync(filterPage1);
        var resultPage2 = await _transactionService.GetTransactionsAsync(filterPage2);

        // Assert - Verify page parameter works correctly
        Assert.Equal(1, resultPage1.CurrentPage);
        Assert.Equal(10, resultPage1.Data.Count);
        
        Assert.Equal(2, resultPage2.CurrentPage);
        Assert.Equal(5, resultPage2.Data.Count); // Only 5 items left on page 2 (15 total - 10 from page 1)
        
        // Verify different transactions returned on different pages
        var page1Ids = resultPage1.Data.Select(t => t.Id).ToList();
        var page2Ids = resultPage2.Data.Select(t => t.Id).ToList();
        Assert.Empty(page1Ids.Intersect(page2Ids)); // No overlap between pages
        
        Console.WriteLine($"✓ Page parameter working: Page 1 has {resultPage1.Data.Count} items, Page 2 has {resultPage2.Data.Count} items");
    }

    [Fact]
    public async Task Task2_5_PageSizeParameter_ShouldWork()
    {
        // Arrange - Task 2.5 requirement: pageSize parameter
        var filter = new TransactionFilterDto
        {
            Page = 1,
            PageSize = 5 // Custom page size
        };

        // Act
        var result = await _transactionService.GetTransactionsAsync(filter);

        // Assert - Verify pageSize parameter works
        Assert.NotNull(result);
        Assert.Equal(5, result.PageSize);
        Assert.Equal(5, result.Data.Count);
        
        Console.WriteLine($"✓ Custom pageSize parameter working: {result.PageSize}");
    }

    [Fact]
    public async Task Task2_5_CurrentPageMetadata_ShouldBeReturned()
    {
        // Arrange - Task 2.5 requirement: currentPage metadata
        var filter = new TransactionFilterDto { Page = 2, PageSize = 7 };

        // Act
        var result = await _transactionService.GetTransactionsAsync(filter);

        // Assert - Verify currentPage metadata
        Assert.NotNull(result);
        Assert.Equal(2, result.CurrentPage);
        
        Console.WriteLine($"✓ CurrentPage metadata returned: {result.CurrentPage}");
    }

    [Fact]
    public async Task Task2_5_TotalPagesMetadata_ShouldBeReturned()
    {
        // Arrange - Task 2.5 requirement: totalPages metadata
        var filter = new TransactionFilterDto { Page = 1, PageSize = 6 }; // 15 items / 6 per page = 3 pages

        // Act
        var result = await _transactionService.GetTransactionsAsync(filter);

        // Assert - Verify totalPages metadata
        Assert.NotNull(result);
        Assert.Equal(3, result.TotalPages); // ceiling(15/6) = 3
        
        Console.WriteLine($"✓ TotalPages metadata returned: {result.TotalPages}");
    }

    [Fact]
    public async Task Task2_5_TotalItemsMetadata_ShouldBeReturned()
    {
        // Arrange - Task 2.5 requirement: totalItems metadata
        var filter = new TransactionFilterDto { Page = 1, PageSize = 8 };

        // Act
        var result = await _transactionService.GetTransactionsAsync(filter);

        // Assert - Verify totalItems metadata
        Assert.NotNull(result);
        Assert.Equal(15, result.TotalItems); // We seeded 15 transactions
        
        Console.WriteLine($"✓ TotalItems metadata returned: {result.TotalItems}");
    }

    [Fact]
    public async Task Task2_5_AllMetadataTogether_ShouldBeComplete()
    {
        // Arrange - Task 2.5 requirement: All metadata together
        var filter = new TransactionFilterDto { Page = 2, PageSize = 4 }; // 15 items, pageSize 4 = 4 pages

        // Act
        var result = await _transactionService.GetTransactionsAsync(filter);

        // Assert - Verify all metadata is present and correct
        Assert.NotNull(result);
        
        // Verify all required metadata from Task 2.5
        Assert.Equal(2, result.CurrentPage);           // currentPage
        Assert.Equal(4, result.TotalPages);            // totalPages (ceiling(15/4) = 4)
        Assert.Equal(15, result.TotalItems);           // totalItems
        Assert.Equal(4, result.PageSize);              // pageSize parameter
        Assert.Equal(4, result.Data.Count);           // Correct number of items returned
        
        // Additional helpful metadata
        Assert.True(result.HasPreviousPage);           // Page 2 should have previous
        Assert.True(result.HasNextPage);               // Page 2 should have next (4 pages total)
        
        Console.WriteLine($"✓ All Task 2.5 requirements verified:");
        Console.WriteLine($"  - page parameter: {result.CurrentPage}");
        Console.WriteLine($"  - pageSize parameter: {result.PageSize} (default would be 10)");
        Console.WriteLine($"  - currentPage metadata: {result.CurrentPage}");
        Console.WriteLine($"  - totalPages metadata: {result.TotalPages}");
        Console.WriteLine($"  - totalItems metadata: {result.TotalItems}");
    }

    [Fact]
    public async Task Task2_5_Requirements2_Compatibility()
    {
        // Arrange - Task 2.5 relates to Requirements 2: Listagem de Transações com Paginação
        var filter = new TransactionFilterDto { Page = 1, PageSize = 10 };

        // Act
        var result = await _transactionService.GetTransactionsAsync(filter);

        // Assert - Verify Requirements 2 compliance
        Assert.NotNull(result);
        
        // Requirement 2.1: tamanho padrão de 10 itens por página
        Assert.Equal(10, result.PageSize);
        
        // Requirement 2.2: ordem decrescente por data
        for (int i = 0; i < result.Data.Count - 1; i++)
        {
            Assert.True(result.Data[i].Date >= result.Data[i + 1].Date,
                "Transactions should be ordered by date descending");
        }
        
        // Requirement 2.3: metadados de paginação
        Assert.True(result.CurrentPage > 0);
        Assert.True(result.TotalPages > 0);
        Assert.True(result.TotalItems >= 0);
        
        // Requirement 2.5: campos retornados
        foreach (var transaction in result.Data)
        {
            Assert.True(transaction.Id > 0);                    // ID
            Assert.NotEqual(default(DateTime), transaction.Date); // data
            Assert.True(transaction.Amount > 0);                // valor
            Assert.True(transaction.CategoryId > 0);            // categoria
            Assert.NotEmpty(transaction.Description);           // descrição
            Assert.True(Enum.IsDefined(transaction.Type));      // tipo
        }
        
        Console.WriteLine("✓ Requirements 2 compliance verified with Task 2.5 implementation");
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}