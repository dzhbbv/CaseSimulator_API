using MediatR;

namespace CaseSimulator.Application.Admin.Commands;

public record CreateCaseItemCommand(
    string Name,
    string ImageUrl,
    string RarityName,
    decimal Price
) : IRequest<Guid>;