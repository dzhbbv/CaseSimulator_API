using MediatR;
namespace CaseSimulator.Application.Admin.Commands;
public record DeleteUserCommand(Guid UserId) : IRequest;