using CaseSimulator.Application.Interfaces;
using CaseSimulator.Domain.Exception;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CaseSimulator.Application.Admin.Commands;

public class AddItemToCaseCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<AddItemToCaseCommand>
{
    public async Task Handle(AddItemToCaseCommand request, CancellationToken cancellationToken)
    {
        var caseEntity = await dbContext.Cases
                             .Include(c => c.CaseContent)
                             .FirstOrDefaultAsync(c => c.Id == request.CaseId, cancellationToken)
                         ?? throw new NotFoundException("Case not found");

        var item = await dbContext.CaseItems
                       .FirstOrDefaultAsync(i => i.Id == request.CaseItemId, cancellationToken)
                   ?? throw new NotFoundException("Item not found");

        caseEntity.AddItem(item, request.DropChance);
        
        var newContentItem = caseEntity.CaseContent.Last();

        await dbContext.CaseContents.AddAsync(newContentItem, cancellationToken);
        
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}