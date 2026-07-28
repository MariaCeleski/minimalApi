using Microsoft.EntityFrameworkCore;
using minimal_api.Dominio.Entidades;

namespace minimal_api.Infraestrutura.Db;

public class DbContexto : DbContext
{
    public DbContexto(DbContextOptions<DbContexto> options) : base(options)
    {
    }

    // DbSets for all entities
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Goal> Goals { get; set; }
    public DbSet<TransactionLimit> TransactionLimits { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        ConfigureTransactions(modelBuilder);
        ConfigureCategories(modelBuilder);
        ConfigureUsers(modelBuilder);
        ConfigureGoals(modelBuilder);
        ConfigureTransactionLimits(modelBuilder);
        
        SeedInitialData(modelBuilder);
    }

    private void ConfigureTransactions(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Amount)
                .HasPrecision(18, 2)
                .IsRequired();
                
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsRequired();
                
            entity.Property(e => e.Date)
                .IsRequired();
                
            entity.Property(e => e.Type)
                .HasConversion<int>()
                .IsRequired();
                
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
                
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Foreign key relationships
            entity.HasOne(t => t.Category)
                .WithMany(c => c.Transactions)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.User)
                .WithMany(u => u.Transactions)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            // Indexes for better query performance
            entity.HasIndex(e => e.Date);
            entity.HasIndex(e => e.CategoryId);
            entity.HasIndex(e => e.Type);
            entity.HasIndex(e => new { e.UserId, e.Date });
        });
    }

    private void ConfigureCategories(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsRequired();
                
            entity.Property(e => e.Description)
                .HasMaxLength(500);
                
            entity.Property(e => e.IconName)
                .HasMaxLength(50)
                .IsRequired();
                
            entity.Property(e => e.Color)
                .HasMaxLength(7)
                .IsRequired();
                
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Unique constraint on category name
            entity.HasIndex(e => e.Name).IsUnique();
        });
    }

    private void ConfigureUsers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsRequired();
                
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsRequired();
                
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsRequired();
                
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
                
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Unique constraint on email
            entity.HasIndex(e => e.Email).IsUnique();
        });
    }

    private void ConfigureGoals(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Goal>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .IsRequired();
                
            entity.Property(e => e.Description)
                .HasMaxLength(1000);
                
            entity.Property(e => e.TargetAmount)
                .HasPrecision(18, 2)
                .IsRequired();
                
            entity.Property(e => e.CurrentAmount)
                .HasPrecision(18, 2)
                .HasDefaultValue(0);
                
            entity.Property(e => e.Status)
                .HasConversion<int>()
                .HasDefaultValue(GoalStatus.Active);
                
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
                
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Foreign key relationship
            entity.HasOne(g => g.User)
                .WithMany(u => u.Goals)
                .HasForeignKey(g => g.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.TargetDate);
        });
    }

    private void ConfigureTransactionLimits(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TransactionLimit>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .IsRequired();
                
            entity.Property(e => e.LimitAmount)
                .HasPrecision(18, 2)
                .IsRequired();
                
            entity.Property(e => e.CurrentSpent)
                .HasPrecision(18, 2)
                .HasDefaultValue(0);
                
            entity.Property(e => e.Period)
                .HasConversion<int>()
                .IsRequired();
                
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
                
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Foreign key relationships
            entity.HasOne(tl => tl.Category)
                .WithMany(c => c.TransactionLimits)
                .HasForeignKey(tl => tl.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(tl => tl.User)
                .WithMany(u => u.TransactionLimits)
                .HasForeignKey(tl => tl.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);

            // Indexes
            entity.HasIndex(e => e.CategoryId);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => new { e.CategoryId, e.Period, e.UserId }).IsUnique();
        });
    }

    private void SeedInitialData(ModelBuilder modelBuilder)
    {
        // Seed default categories with icons and colors as mentioned in requirements
        modelBuilder.Entity<Category>().HasData(
            new Category 
            { 
                Id = 1, 
                Name = "Alimentação", 
                Description = "Gastos com comida e bebidas", 
                IconName = "utensils", 
                Color = "#FF6B6B",
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Category 
            { 
                Id = 2, 
                Name = "Transporte", 
                Description = "Gastos com locomoção", 
                IconName = "car", 
                Color = "#4ECDC4",
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Category 
            { 
                Id = 3, 
                Name = "Lazer", 
                Description = "Entretenimento e diversão", 
                IconName = "gamepad", 
                Color = "#45B7D1",
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Category 
            { 
                Id = 4, 
                Name = "Saúde", 
                Description = "Gastos com saúde e medicamentos", 
                IconName = "heart", 
                Color = "#96CEB4",
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Category 
            { 
                Id = 5, 
                Name = "Educação", 
                Description = "Cursos, livros e material educacional", 
                IconName = "book", 
                Color = "#FECA57",
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Category 
            { 
                Id = 6, 
                Name = "Utilitários", 
                Description = "Contas básicas (água, luz, internet)", 
                IconName = "home", 
                Color = "#FF9FF3",
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Category 
            { 
                Id = 7, 
                Name = "Investimento", 
                Description = "Aplicações e investimentos", 
                IconName = "trending-up", 
                Color = "#54A0FF",
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Category 
            { 
                Id = 8, 
                Name = "Outros", 
                Description = "Gastos diversos não categorizados", 
                IconName = "more-horizontal", 
                Color = "#A4B0BE",
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}