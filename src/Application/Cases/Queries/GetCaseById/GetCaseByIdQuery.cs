using MediatR;

namespace CaseSimulator.Application.Cases.Queries.GetCaseById;

public class GetCaseByIdQuery : IRequest<CaseDetailDto>
{
    public Guid CaseId { get; init; }
}