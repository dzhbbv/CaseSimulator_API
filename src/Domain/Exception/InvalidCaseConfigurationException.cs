namespace CaseSimulator.Domain.Exception;

public class InvalidCaseConfigurationException : DomainException
{
    public InvalidCaseConfigurationException() : base("The total drop chances for the case are invalid.") { }
}