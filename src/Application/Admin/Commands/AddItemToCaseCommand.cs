using MediatR;

namespace CaseSimulator.Application.Admin.Commands;

public record AddItemToCaseCommand(
    Guid CaseId,
    Guid CaseItemId,
    decimal DropChance
) : IRequest;