namespace CaseSimulator.Domain.Exception;

public class AlreadyExistingException : DomainException
{
    public AlreadyExistingException(string message) : base(message) { }
}