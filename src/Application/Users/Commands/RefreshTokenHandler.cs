using CaseSimulator.Application.Interfaces;
using CaseSimulator.Domain.Exception;
using CaseSimulator.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CaseSimulator.Application.Users.Commands;

public class RefreshTokenHandler(
    IApplicationDbContext dbContext,
    IJwtTokenService jwtTokenService) : IRequestHandler<RefreshTokenCommand, AuthResult>
{
    public async Task<AuthResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var existing = await dbContext.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == request.RefreshToken, cancellationToken);

        if (existing is null || !existing.IsActive)
            throw new InvalidCredentialsException();

        existing.Revoke();

        var newAccessToken = jwtTokenService.GenerateToken(existing.User);
        var newRefreshToken = jwtTokenService.GenerateRefreshToken();

        var newRefreshTokenEntity = new RefreshToken(
            existing.UserId,
            newRefreshToken,
            DateTime.UtcNow.AddDays(7));

        await dbContext.RefreshTokens.AddAsync(newRefreshTokenEntity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new AuthResult(newAccessToken, newRefreshToken);
    }
}