namespace CaseSimulator.Domain.Exception;

public class InvalidCredentialsException : DomainException
{
    public InvalidCredentialsException() : base("Invalid email or password") { }
}