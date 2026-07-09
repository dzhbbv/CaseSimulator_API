using MediatR;
namespace CaseSimulator.Application.Admin.Commands;
public record DeleteCaseItemCommand(Guid CaseItemId) : IRequest;