using MediatR;

namespace CaseSimulator.Application.Cases.Queries.GetAllCases;

public class GetAllCasesQuery : IRequest<IReadOnlyCollection<CaseDto>>
{

}