using MediatR;

namespace CaseSimulator.Application.Users.Commands;

public record RefreshTokenCommand(
    string RefreshToken
) : IRequest<AuthResult>;