using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace minimal_api.Dominio.Entidades;

[Table("Users")]
public class User
{
    [Key]
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Email é obrigatório")]
    [MaxLength(255, ErrorMessage = "Email deve ter no máximo 255 caracteres")]
    [EmailAddress(ErrorMessage = "Email deve ter formato válido")]
    public string Email { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Hash da senha é obrigatório")]
    [MaxLength(255, ErrorMessage = "Hash da senha deve ter no máximo 255 caracteres")]
    public string PasswordHash { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Nome é obrigatório")]
    [MaxLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
    [MinLength(2, ErrorMessage = "Nome deve ter pelo menos 2 caracteres")]
    public string Name { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation Properties
    public virtual ICollection<Transaction> Transactions { get; set; } = [];
    public virtual ICollection<Goal> Goals { get; set; } = [];
    public virtual ICollection<TransactionLimit> TransactionLimits { get; set; } = [];
    
    // Business Logic Methods
    public bool IsValidEmail()
    {
        if (string.IsNullOrWhiteSpace(Email))
            return false;
            
        // RFC 5322 simplified validation
        var emailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
        return emailRegex.IsMatch(Email);
    }
    
    public void UpdateTimestamp()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}