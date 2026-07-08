using System.Security.Authentication;
using CaseSimulator.Application.Interfaces;
using CaseSimulator.Domain.Exception;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CaseSimulator.Application.Users.Commands;

public class LoginCommandHandler(
    IApplicationDbContext dbContext,
    IPasswordService passwordService,
    IJwtTokenService jwtTokenService) : IRequestHandler<LoginCommand, AuthResult>
{
    public async Task<AuthResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email.Trim().ToLowerInvariant(), cancellationToken);

        if (user is null)
            throw new InvalidCredentialException("Invalid credentials");

        if (!passwordService.Verify(request.Password, user.PasswordHash))
            throw new InvalidCredentialException("Invalid credentials");

        var accessToken = jwtTokenService.GenerateToken(user);
        var refreshToken = jwtTokenService.GenerateRefreshToken();

        return new AuthResult(accessToken, refreshToken);
    }
}