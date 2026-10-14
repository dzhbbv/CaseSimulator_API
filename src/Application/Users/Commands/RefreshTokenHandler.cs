using CaseSimulator.Application.Interfaces;
using CaseSimulator.Application.Users;
using CaseSimulator.Domain.Entities;
using CaseSimulator.Domain.Exception;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CaseSimulator.Application.Users.Commands;

public class RefreshTokenHandler(
    IApplicationDbContext dbContext,
    IJwtTokenService jwtTokenService) : IRequestHandler<RefreshTokenCommand, AuthResult>
{
    public async Task<AuthResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            throw new InvalidCredentialsException();

        var tokenHash = RefreshTokenHasher.Hash(request.RefreshToken);
        var existing = await dbContext.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == tokenHash, cancellationToken);

        if (existing is null || !existing.IsActive)
            throw new InvalidCredentialsException();

        existing.Revoke();

        var rawRefreshToken = jwtTokenService.GenerateRefreshToken();
        var accessToken = jwtTokenService.GenerateToken(existing.User);
        var replacement = new RefreshToken(
            existing.UserId,
            RefreshTokenHasher.Hash(rawRefreshToken),
            DateTime.UtcNow.AddDays(7));

        await dbContext.RefreshTokens.AddAsync(replacement, cancellationToken);

        try
        {
            // SaveChanges is transactional: revoke and replacement commit together.
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            // Another request already rotated this token.
            throw new InvalidCredentialsException();
        }

        return new AuthResult(accessToken, rawRefreshToken);
    }
}
