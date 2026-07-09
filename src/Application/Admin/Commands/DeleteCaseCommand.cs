using MediatR;
namespace CaseSimulator.Application.Admin.Commands;
public record DeleteCaseCommand(Guid CaseId) : IRequest;