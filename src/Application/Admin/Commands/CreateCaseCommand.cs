using MediatR;

namespace CaseSimulator.Application.Admin.Commands;

public record CreateCaseCommand(
    string Name,
    string ImageUrl,
    decimal Price
) : IRequest<Guid>;