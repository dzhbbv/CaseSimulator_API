using MediatR;

namespace CaseSimulator.Application.Admin.Commands;

public record RemoveItemFromCaseCommand(
    Guid CaseId,
    Guid ItemId) : IRequest;