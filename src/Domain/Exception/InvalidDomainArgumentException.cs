namespace CaseSimulator.Domain.Exception;

public class InvalidDomainArgumentException : DomainException
{
    public InvalidDomainArgumentException(string message) : base(message) { }
}