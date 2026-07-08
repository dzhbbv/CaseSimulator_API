using MediatR;

namespace CaseSimulator.Application.Users.Commands;

public record RegisterUserCommand(
    string Username,
    string Email,
    string Password,
    string ClientSeed
) : IRequest<Guid>;