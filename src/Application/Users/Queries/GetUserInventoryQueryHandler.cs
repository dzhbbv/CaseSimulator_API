using CaseSimulator.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CaseSimulator.Application.Users.Queries;

public class GetUserInventoryQueryHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService) : IRequestHandler<GetUserInventoryQuery, IReadOnlyCollection<InventoryItemDto>>
{
    public async Task<IReadOnlyCollection<InventoryItemDto>> Handle(GetUserInventoryQuery request, CancellationToken cancellationToken)
    {
        var items = await dbContext.InventoryItems
            .Include(i => i.CaseItem)
            .Where(i => i.UserId == currentUserService.UserId)
            .Select(i => new InventoryItemDto(
                i.Id,
                i.CaseItem.Name,
                i.CaseItem.Price.Amount,
                i.CaseItem.ImageUrl,
                i.CaseItem.Rarity.Name,
                i.CaseItem.Rarity.Color))
            .ToListAsync(cancellationToken);

        return items.AsReadOnly();
    }
}