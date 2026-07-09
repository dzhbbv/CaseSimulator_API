using CaseSimulator.Application.Interfaces;
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
                         ?? throw new Exception("Case not found");

        var item = await dbContext.CaseItems
                       .FirstOrDefaultAsync(i => i.Id == request.CaseItemId, cancellationToken)
                   ?? throw new Exception("Item not found");

        caseEntity.AddItem(item, request.DropChance);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}