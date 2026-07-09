using CaseSimulator.Application.Interfaces;
using CaseSimulator.Domain.Entities;
using CaseSimulator.Domain.ValueObjects;
using MediatR;

namespace CaseSimulator.Application.Admin.Commands;

public class CreateCaseItemCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<CreateCaseItemCommand, Guid>
{
    public async Task<Guid> Handle(CreateCaseItemCommand request, CancellationToken cancellationToken)
    {
        var rarity = Rarity.FromName(request.RarityName);
        var item = new CaseItem(request.Name, request.ImageUrl, rarity, new Money(request.Price));
        await dbContext.CaseItems.AddAsync(item, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return item.Id;
    }
}