using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace minimal_api.Dominio.Entidades;

[Table("TransactionLimits")]
public class TransactionLimit
{
    [Key]
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Nome do limite é obrigatório")]
    [MaxLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
    [MinLength(3, ErrorMessage = "Nome deve ter pelo menos 3 caracteres")]
    public string Name { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Valor do limite é obrigatório")]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Valor do limite deve ser maior que zero")]
    public decimal LimitAmount { get; set; }
    
    [Required(ErrorMessage = "Período do limite é obrigatório")]
    public LimitPeriod Period { get; set; }
    
    [Required(ErrorMessage = "Categoria é obrigatória")]
    [Range(1, int.MaxValue, ErrorMessage = "Categoria deve ser selecionada")]
    public int CategoryId { get; set; }
    
    public int? UserId { get; set; } // Nullable for optional authentication
    
    public bool IsActive { get; set; } = true;
    
    [Column(TypeName = "decimal(18,2)")]
    [Range(0, double.MaxValue, ErrorMessage = "Valor gasto não pode ser negativo")]
    public decimal CurrentSpent { get; set; } = 0;
    
    public DateTime PeriodStart { get; set; }
    
    public DateTime PeriodEnd { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Calculated Properties
    [NotMapped]
    public decimal RemainingAmount => Math.Max(LimitAmount - CurrentSpent, 0);
    
    [NotMapped]
    public decimal UsagePercentage => LimitAmount > 0 ? CurrentSpent / LimitAmount * 100 : 0;
    
    [NotMapped]
    public bool IsExceeded => CurrentSpent > LimitAmount;
    
    [NotMapped]
    public bool IsNearLimit => UsagePercentage >= 80;
    
    [NotMapped]
    public bool IsWarningZone => UsagePercentage >= 80 && UsagePercentage < 100;
    
    // Navigation Properties
    [ForeignKey("CategoryId")]
    public virtual Category Category { get; set; } = null!;
    
    [ForeignKey("UserId")]
    public virtual User? User { get; set; }
    
    // Business Logic Methods
    public bool IsValidPeriod()
    {
        return PeriodEnd > PeriodStart;
    }
    
    public bool IsCurrentPeriod()
    {
        var now = DateTime.Now.Date;
        return now >= PeriodStart.Date && now <= PeriodEnd.Date;
    }
    
    public void AddSpending(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Valor de gasto deve ser maior que zero");
            
        CurrentSpent += amount;
        UpdateTimestamp();
    }
    
    public void RemoveSpending(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Valor de remoção deve ser maior que zero");
            
        CurrentSpent = Math.Max(0, CurrentSpent - amount);
        UpdateTimestamp();
    }
    
    public void ResetPeriod()
    {
        CurrentSpent = 0;
        SetCurrentPeriod();
        UpdateTimestamp();
    }
    
    public void SetCurrentPeriod()
    {
        var now = DateTime.Now.Date;
        
        switch (Period)
        {
            case LimitPeriod.Daily:
                PeriodStart = now;
                PeriodEnd = now;
                break;
            case LimitPeriod.Weekly:
                var startOfWeek = now.AddDays(-(int)now.DayOfWeek);
                PeriodStart = startOfWeek;
                PeriodEnd = startOfWeek.AddDays(6);
                break;
            case LimitPeriod.Monthly:
                PeriodStart = new DateTime(now.Year, now.Month, 1);
                PeriodEnd = PeriodStart.AddMonths(1).AddDays(-1);
                break;
            case LimitPeriod.Yearly:
                PeriodStart = new DateTime(now.Year, 1, 1);
                PeriodEnd = new DateTime(now.Year, 12, 31);
                break;
        }
    }
    
    public void UpdateTimestamp()
    {
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void Activate()
    {
        IsActive = true;
        UpdateTimestamp();
    }
    
    public void Deactivate()
    {
        IsActive = false;
        UpdateTimestamp();
    }
    
    /// <summary>
    /// Retorna o tipo de notificação baseado no uso atual
    /// </summary>
    /// <returns>Null se não há notificação, "warning" se 80%+, "alert" se 100%+</returns>
    public string? GetNotificationType()
    {
        if (!IsActive) return null;
        
        if (IsExceeded) return "alert";
        if (IsNearLimit) return "warning";
        
        return null;
    }
}

public enum LimitPeriod
{
    Daily = 1,
    Weekly = 2,
    Monthly = 3,
    Yearly = 4
}