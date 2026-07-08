using MediatR;

namespace CaseSimulator.Application.Users.Queries;

public record GetUserInventoryQuery : IRequest<IReadOnlyCollection<InventoryItemDto>>;