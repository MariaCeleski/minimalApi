using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace minimal_api.Dominio.Entidades;

[Table("Transactions")]
public class Transaction
{
    [Key]
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Data é obrigatória")]
    public DateTime Date { get; set; }
    
    [Required(ErrorMessage = "Valor é obrigatório")]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser maior que zero")]
    public decimal Amount { get; set; }
    
    [Required(ErrorMessage = "Descrição é obrigatória")]
    [MaxLength(255, ErrorMessage = "Descrição deve ter no máximo 255 caracteres")]
    [MinLength(1, ErrorMessage = "Descrição deve ter pelo menos 1 caractere")]
    public string Description { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Tipo de transação é obrigatório")]
    public TransactionType Type { get; set; }
    
    [Required(ErrorMessage = "Categoria é obrigatória")]
    [Range(1, int.MaxValue, ErrorMessage = "Categoria deve ser selecionada")]
    public int CategoryId { get; set; }
    
    public int? UserId { get; set; } // Nullable for optional authentication
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation Properties
    [ForeignKey("CategoryId")]
    public virtual Category Category { get; set; } = null!;
    
    [ForeignKey("UserId")]
    public virtual User? User { get; set; }
    
    // Business Logic Methods
    public bool IsValidDate()
    {
        // Data não pode ser futura (baseado no Requirement 1)
        return Date.Date <= DateTime.Now.Date;
    }
    
    public bool IsIncome()
    {
        return Type == TransactionType.Income;
    }
    
    public bool IsExpense()
    {
        return Type == TransactionType.Expense;
    }
    
    public decimal GetSignedAmount()
    {
        return Type == TransactionType.Income ? Amount : -Amount;
    }
    
    public void UpdateTimestamp()
    {
        UpdatedAt = DateTime.UtcNow;
    }
    
    public bool HasValidDescription()
    {
        return !string.IsNullOrWhiteSpace(Description) && 
               Description.Length <= 255 && 
               !ContainsHarmfulCharacters(Description);
    }
    
    private static bool ContainsHarmfulCharacters(string input)
    {
        // Verificar caracteres especiais prejudiciais mencionados no Requirement 1
        var harmfulChars = new[] { '<', '>', '"', '\'', '&', ';', '(', ')', '{', '}', '[', ']' };
        return harmfulChars.Any(input.Contains);
    }
}

public enum TransactionType
{
    Income = 1,
    Expense = 2
}