using MediatR;

namespace CaseSimulator.Application.Users.Commands;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<AuthResult>;