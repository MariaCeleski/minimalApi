using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace minimal_api.Dominio.Entidades;

[Table("TransactionLimits")]
public class TransactionLimit
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal LimitAmount { get; set; }
    
    [Required]
    public LimitPeriod Period { get; set; }
    
    [Required]
    public int CategoryId { get; set; }
    
    public int? UserId { get; set; } // Nullable for optional authentication
    
    public bool IsActive { get; set; } = true;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal CurrentSpent { get; set; } = 0;
    
    public DateTime PeriodStart { get; set; }
    
    public DateTime PeriodEnd { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Calculated Properties
    [NotMapped]
    public decimal RemainingAmount => LimitAmount - CurrentSpent;
    
    [NotMapped]
    public decimal UsagePercentage => LimitAmount > 0 ? (CurrentSpent / LimitAmount) * 100 : 0;
    
    [NotMapped]
    public bool IsExceeded => CurrentSpent > LimitAmount;
    
    [NotMapped]
    public bool IsNearLimit => UsagePercentage >= 80;
    
    // Navigation Properties
    [ForeignKey("CategoryId")]
    public virtual Category Category { get; set; } = null!;
    
    [ForeignKey("UserId")]
    public virtual User? User { get; set; }
}

public enum LimitPeriod
{
    Daily = 1,
    Weekly = 2,
    Monthly = 3,
    Yearly = 4
}