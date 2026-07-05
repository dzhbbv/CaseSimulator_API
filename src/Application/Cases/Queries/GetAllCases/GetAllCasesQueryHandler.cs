using CaseSimulator.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using MediatR;

namespace CaseSimulator.Application.Cases.Queries.GetAllCases;

public class GetAllCasesQueryHandler(IApplicationDbContext dbContext) : IRequestHandler<GetAllCasesQuery, IReadOnlyCollection<CaseDto>>
{
    public async Task<IReadOnlyCollection<CaseDto>> Handle(GetAllCasesQuery request, CancellationToken cancellationToken)
    {
        var caseEntities = await dbContext.Cases
            .Include(c => c.CaseContent)
            .Select(c => new CaseDto(c))
            .ToListAsync(cancellationToken);
        return caseEntities.AsReadOnly();
    }
}