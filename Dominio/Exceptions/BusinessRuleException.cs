namespace minimal_api.Dominio.Exceptions;

/// <summary>
/// Exception thrown when business rules are violated
/// Supports Requirements 18, 19, 20 - Goals, limits, and data integrity
/// </summary>
public class BusinessRuleException : Exception
{
    public string BusinessRule { get; }
    public string Context { get; }

    public BusinessRuleException() : base("Business rule violation")
    {
        BusinessRule = string.Empty;
        Context = string.Empty;
    }

    public BusinessRuleException(string message) : base(message)
    {
        BusinessRule = string.Empty;
        Context = string.Empty;
    }

    public BusinessRuleException(string message, Exception innerException) : base(message, innerException)
    {
        BusinessRule = string.Empty;
        Context = string.Empty;
    }

    public BusinessRuleException(string businessRule, string context, string message) : base(message)
    {
        BusinessRule = businessRule;
        Context = context;
    }

    /// <summary>
    /// Creates a BusinessRuleException for data integrity violation (Requirements 20)
    /// </summary>
    public static BusinessRuleException ForDataIntegrityViolation(string details)
    {
        return new BusinessRuleException("DataIntegrity", "Balance", $"Integridade de dados violada: {details}");
    }

    /// <summary>
    /// Creates a BusinessRuleException for goal limit exceeded (Requirements 18)
    /// </summary>
    public static BusinessRuleException ForGoalLimitExceeded(string goalName, decimal currentAmount, decimal targetAmount)
    {
        return new BusinessRuleException("GoalLimit", goalName, 
            $"Meta '{goalName}' já foi atingida. Valor atual: {currentAmount:C}, Meta: {targetAmount:C}");
    }

    /// <summary>
    /// Creates a BusinessRuleException for transaction limit exceeded (Requirements 19)
    /// </summary>
    public static BusinessRuleException ForTransactionLimitExceeded(string categoryName, decimal currentAmount, decimal limitAmount)
    {
        return new BusinessRuleException("TransactionLimit", categoryName, 
            $"Limite de gastos para '{categoryName}' excedido. Valor atual: {currentAmount:C}, Limite: {limitAmount:C}");
    }

    /// <summary>
    /// Creates a BusinessRuleException for invalid balance calculation (Requirements 5, 20)
    /// </summary>
    public static BusinessRuleException ForInvalidBalanceCalculation(decimal calculatedBalance, decimal storedBalance)
    {
        return new BusinessRuleException("BalanceCalculation", "Balance", 
            $"Saldo calculado ({calculatedBalance:C}) não confere com o saldo armazenado ({storedBalance:C})");
    }

    public override string ToString()
    {
        var baseString = base.ToString();
        if (!string.IsNullOrEmpty(BusinessRule) || !string.IsNullOrEmpty(Context))
        {
            return $"{baseString}\nRegra de negócio: {BusinessRule}\nContexto: {Context}";
        }
        return baseString;
    }
}