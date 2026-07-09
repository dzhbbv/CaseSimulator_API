namespace CaseSimulator.Domain.Exception;

public class NotFoundException : DomainException
{
    public NotFoundException(string message) : base(message) { }
}