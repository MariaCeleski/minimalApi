using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace minimal_api.Dominio.Entidades;

[Table("Goals")]
public class Goal
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TargetAmount { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal CurrentAmount { get; set; } = 0;
    
    [Required]
    public DateTime TargetDate { get; set; }
    
    public GoalStatus Status { get; set; } = GoalStatus.Active;
    
    [Required]
    public int UserId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Calculated Properties
    [NotMapped]
    public decimal ProgressPercentage => TargetAmount > 0 ? Math.Min((CurrentAmount / TargetAmount) * 100, 100) : 0;
    
    [NotMapped]
    public bool IsCompleted => CurrentAmount >= TargetAmount;
    
    // Navigation Properties
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
}

public enum GoalStatus
{
    Active = 1,
    Completed = 2,
    Cancelled = 3,
    Paused = 4
}