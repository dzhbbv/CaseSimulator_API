using CaseSimulator.Application.Interfaces;
using CaseSimulator.Domain.Exception;
using CaseSimulator.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CaseSimulator.Application.Admin.Commands;

public class UpdateCaseItemCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<UpdateCaseItemCommand>
{
    public async Task Handle(UpdateCaseItemCommand request, CancellationToken cancellationToken)
    {
        var caseItem = await dbContext.CaseItems
                             .FirstOrDefaultAsync(c => c.Id == request.CaseItemId, cancellationToken)
                         ?? throw new NotFoundException("Case item not found");
        
        var rarity = Rarity.FromName(request.Rarity);
        caseItem.Update(request.Name, request.ImageUrl, rarity, new Money(request.Price));
        
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}