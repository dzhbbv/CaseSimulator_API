using BCrypt.Net;
using CaseSimulator.Application.Interfaces;

namespace CaseSimulator.Infrastructure.Services;

public class BCryptPasswordService : IPasswordService
{
    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool Verify(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}