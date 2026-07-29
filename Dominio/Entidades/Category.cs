using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace minimal_api.Dominio.Entidades;

[Table("Categories")]
public class Category
{
    [Key]
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Nome da categoria é obrigatório")]
    [MaxLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
    [MinLength(2, ErrorMessage = "Nome deve ter pelo menos 2 caracteres")]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(500, ErrorMessage = "Descrição deve ter no máximo 500 caracteres")]
    public string? Description { get; set; }
    
    [Required(ErrorMessage = "Nome do ícone é obrigatório")]
    [MaxLength(50, ErrorMessage = "Nome do ícone deve ter no máximo 50 caracteres")]
    public string IconName { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Cor é obrigatória")]
    [MaxLength(7, ErrorMessage = "Cor deve ter no máximo 7 caracteres (formato #RRGGBB)")]
    [RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "Cor deve estar no formato hexadecimal (#RRGGBB)")]
    public string Color { get; set; } = string.Empty;
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation Properties
    public virtual ICollection<Transaction> Transactions { get; set; } = [];
    public virtual ICollection<TransactionLimit> TransactionLimits { get; set; } = [];
    
    // Business Logic Methods
    public bool IsValidColor()
    {
        if (string.IsNullOrWhiteSpace(Color))
            return false;
            
        return System.Text.RegularExpressions.Regex.IsMatch(Color, @"^#[0-9A-Fa-f]{6}$");
    }
    
    public void Deactivate()
    {
        IsActive = false;
    }
    
    public void Activate()
    {
        IsActive = true;
    }
}