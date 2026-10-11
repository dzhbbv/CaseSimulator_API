using CaseSimulator.Application.Interfaces;
using CaseSimulator.Domain.Entities;
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
                             .FirstOrDefaultAsync(c => c.Id == request.CaseId, cancellationToken)
                         ?? throw new NotFoundException("Case not found");

        var item = await dbContext.CaseItems
                       .FirstOrDefaultAsync(i => i.Id == request.CaseItemId, cancellationToken)
                   ?? throw new NotFoundException("Item not found");

        var existingContents = await dbContext.CaseContents
            .Where(cc => cc.CaseId == request.CaseId)
            .ToListAsync(cancellationToken);

        if (existingContents.Any(c => c.CaseItemId == request.CaseItemId))
            throw new AlreadyExistingException("Item already in case");

        decimal currentTotal = existingContents.Sum(c => c.DropChance);
        if (currentTotal + request.DropChance > 1.0m)
            throw new InvalidCaseConfigurationException();

        var newContent = new CaseContent(caseEntity.Id, item.Id, item, request.DropChance);
        await dbContext.CaseContents.AddAsync(newContent, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}