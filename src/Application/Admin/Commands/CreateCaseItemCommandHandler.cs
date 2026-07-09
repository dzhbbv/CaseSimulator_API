using CaseSimulator.Application.Interfaces;
using CaseSimulator.Domain.Entities;
using CaseSimulator.Domain.ValueObjects;
using CaseSimulator.Domain.Exception;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CaseSimulator.Application.Admin.Commands;

public class CreateCaseItemCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<CreateCaseItemCommand, Guid>
{
    public async Task<Guid> Handle(CreateCaseItemCommand request, CancellationToken cancellationToken)
    {
        var normalizedName = request.Name.Trim().ToLowerInvariant();
        
        var exists = await dbContext.CaseItems
            .AnyAsync(i => i.Name == normalizedName, cancellationToken);
            
        if (exists)
            throw new AlreadyExistingException($"Item with name '{request.Name}' already exists.");

        var rarity = Rarity.FromName(request.RarityName);
        var item = new CaseItem(request.Name, request.ImageUrl, rarity, new Money(request.Price));
        
        await dbContext.CaseItems.AddAsync(item, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return item.Id;
    }
}