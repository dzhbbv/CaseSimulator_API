using CaseSimulator.Application.Interfaces;
using CaseSimulator.Domain.Exception;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace CaseSimulator.Application.Admin.Commands;
public class DeleteCaseItemCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<DeleteCaseItemCommand>
{
    public async Task Handle(DeleteCaseItemCommand request, CancellationToken cancellationToken)
    {
        var item = await dbContext.CaseItems
                       .FirstOrDefaultAsync(i => i.Id == request.CaseItemId, cancellationToken)
                   ?? throw new NotFoundException("CaseItem not found");

        var caseContents = await dbContext.CaseContents
            .Where(cc => cc.CaseItemId == request.CaseItemId)
            .ToListAsync(cancellationToken);

        dbContext.CaseContents.RemoveRange(caseContents);
        dbContext.CaseItems.Remove(item);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}