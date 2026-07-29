using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace minimal_api.Dominio.Entidades;

[Table("Goals")]
public class Goal
{
    [Key]
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Nome da meta é obrigatório")]
    [MaxLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
    [MinLength(3, ErrorMessage = "Nome deve ter pelo menos 3 caracteres")]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(1000, ErrorMessage = "Descrição deve ter no máximo 1000 caracteres")]
    public string? Description { get; set; }
    
    [Required(ErrorMessage = "Valor alvo é obrigatório")]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Valor alvo deve ser maior que zero")]
    public decimal TargetAmount { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    [Range(0, double.MaxValue, ErrorMessage = "Valor atual não pode ser negativo")]
    public decimal CurrentAmount { get; set; } = 0;
    
    [Required(ErrorMessage = "Data limite é obrigatória")]
    public DateTime TargetDate { get; set; }
    
    public GoalStatus Status { get; set; } = GoalStatus.Active;
    
    [Required(ErrorMessage = "Usuário é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "ID do usuário deve ser válido")]
    public int UserId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Calculated Properties
    [NotMapped]
    public decimal ProgressPercentage => TargetAmount > 0 ? Math.Min((CurrentAmount / TargetAmount) * 100, 100) : 0;
    
    [NotMapped]
    public bool IsCompleted => CurrentAmount >= TargetAmount;
    
    [NotMapped]
    public bool IsOverdue => !IsCompleted && DateTime.Now.Date > TargetDate.Date;
    
    [NotMapped]
    public TimeSpan RemainingTime => TargetDate.Date - DateTime.Now.Date;
    
    [NotMapped]
    public decimal RemainingAmount => Math.Max(TargetAmount - CurrentAmount, 0);
    
    // Navigation Properties
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
    
    // Business Logic Methods
    public bool IsValidTargetDate()
    {
        return TargetDate.Date > DateTime.Now.Date;
    }
    
    public void UpdateProgress(decimal amount)
    {
        if (amount < 0) 
            throw new ArgumentException("Valor para atualizar progresso não pode ser negativo");
            
        CurrentAmount += amount;
        UpdateTimestamp();
        
        if (IsCompleted && Status == GoalStatus.Active)
        {
            Status = GoalStatus.Completed;
        }
    }
    
    public void UpdateTimestamp()
    {
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void SetCompleted()
    {
        Status = GoalStatus.Completed;
        UpdateTimestamp();
    }
    
    public void SetCancelled()
    {
        Status = GoalStatus.Cancelled;
        UpdateTimestamp();
    }
    
    public void SetPaused()
    {
        Status = GoalStatus.Paused;
        UpdateTimestamp();
    }
    
    public void SetActive()
    {
        Status = GoalStatus.Active;
        UpdateTimestamp();
    }
}

public enum GoalStatus
{
    Active = 1,
    Completed = 2,
    Cancelled = 3,
    Paused = 4
}