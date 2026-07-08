using CaseSimulator.Domain.Entities;

namespace CaseSimulator.Application.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(User user);
    string GenerateRefreshToken();
}