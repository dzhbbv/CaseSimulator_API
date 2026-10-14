using CaseSimulator.Application.Interfaces;
using CaseSimulator.Domain.Exception;
using CaseSimulator.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CaseSimulator.Application.Admin.Commands;

public class UpdateCaseCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<UpdateCaseCommand>
{
    public async Task Handle(UpdateCaseCommand request, CancellationToken cancellationToken)
    {
        var caseEntity = await dbContext.Cases
                             .FirstOrDefaultAsync(c => c.Id == request.CaseId, cancellationToken)
                         ?? throw new NotFoundException("Case not found");
        
        caseEntity.Update(request.Name, request.ImageUrl, new Money(request.Price));
        
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}