namespace CaseSimulator.Domain.Exception;

public class InsufficientBalanceException : DomainException
{
    public InsufficientBalanceException(decimal current, decimal required)
        : base($"Insufficient balance. Current: {current:F2}, required: {required:F2}")
    {
        Current = current;
        Required = required;
    }
    public decimal Current { get; }
    public decimal Required { get; }
}