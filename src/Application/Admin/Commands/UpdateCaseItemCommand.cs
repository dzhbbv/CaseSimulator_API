using MediatR;

namespace CaseSimulator.Application.Admin.Commands;

public record UpdateCaseItemCommand (
    Guid CaseItemId,
    string Name,
    string ImageUrl,
    string Rarity,
    decimal Price
) : IRequest;