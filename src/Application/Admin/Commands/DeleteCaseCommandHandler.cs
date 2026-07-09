using CaseSimulator.Application.Interfaces;
using CaseSimulator.Domain.Exception;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace CaseSimulator.Application.Admin.Commands;
public class DeleteCaseCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<DeleteCaseCommand>
{
    public async Task Handle(DeleteCaseCommand request, CancellationToken cancellationToken)
    {
        var caseEntity = await dbContext.Cases
                             .FirstOrDefaultAsync(c => c.Id == request.CaseId, cancellationToken)
                         ?? throw new NotFoundException("Case not found");
        dbContext.Cases.Remove(caseEntity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}