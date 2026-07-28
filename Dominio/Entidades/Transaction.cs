using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace minimal_api.Dominio.Entidades;

[Table("Transactions")]
public class Transaction
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public DateTime Date { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    public TransactionType Type { get; set; }
    
    [Required]
    public int CategoryId { get; set; }
    
    public int? UserId { get; set; } // Nullable for optional authentication
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation Properties
    [ForeignKey("CategoryId")]
    public virtual Category Category { get; set; } = null!;
    
    [ForeignKey("UserId")]
    public virtual User? User { get; set; }
}

public enum TransactionType
{
    Income = 1,
    Expense = 2
}