using CaseSimulator.Application.Interfaces;
using CaseSimulator.Domain.Exception; // Предполагается наличие NotFoundException : DomainException
using Microsoft.EntityFrameworkCore;
using MediatR;

namespace CaseSimulator.Application.Cases.Queries.GetCaseById;

public class GetCaseByIdQueryHandler(IApplicationDbContext dbContext) : IRequestHandler<GetCaseByIdQuery, CaseDetailDto>
{
    public async Task<CaseDetailDto> Handle(GetCaseByIdQuery request, CancellationToken cancellationToken)
    {
        var caseEntity = await dbContext.Cases
            .Include(c => c.CaseContent)
            .ThenInclude(cc => cc.CaseItem)
            .FirstOrDefaultAsync(c => c.Id == request.CaseId, cancellationToken);
            
        if (caseEntity == null)
            throw new NotFoundException($"Case with ID {request.CaseId} was not found");
            
        return new CaseDetailDto(caseEntity);
    }
}