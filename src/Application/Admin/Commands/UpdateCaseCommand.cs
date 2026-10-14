using MediatR;

namespace CaseSimulator.Application.Admin.Commands;

public record UpdateCaseCommand (
    Guid CaseId,
    string Name,
    string ImageUrl,
    decimal Price
) : IRequest;