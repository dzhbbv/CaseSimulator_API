using CaseSimulator.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace CaseSimulator.Application.Users.Queries;
public class GetSaleHistoryQueryHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetSaleHistoryQuery, IReadOnlyCollection<SaleHistoryDto>>
{
    public async Task<IReadOnlyCollection<SaleHistoryDto>> Handle(GetSaleHistoryQuery request, CancellationToken cancellationToken)
    {
        var items = await dbContext.SaleItems
            .Where(s => s.UserId == currentUserService.UserId)
            .Select(s => new SaleHistoryDto(s.Id, s.CaseItemId, s.Price.Amount, s.SoldDate))
            .ToListAsync(cancellationToken);
        return items.AsReadOnly();
    }
}