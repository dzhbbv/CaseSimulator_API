using CaseSimulator.Application.Interfaces;
using CaseSimulator.Domain.Entities;
using CaseSimulator.Domain.Exception;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CaseSimulator.Application.Users.Commands;

public class SaleItemCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService usr) : IRequestHandler<SaleItemCommand, Transaction>
{
    private const int MaxRetries = 3;

    public async Task<Transaction> Handle(SaleItemCommand request, CancellationToken cancellationToken)
    {
        for (int attempt = 0; attempt < MaxRetries; attempt++)
        {
            try
            {
                return await TryHandle(request, cancellationToken);
            }
            catch (DbUpdateConcurrencyException ex) when (attempt < MaxRetries - 1)
            {
                foreach (var entry in ex.Entries)
                    await entry.ReloadAsync(cancellationToken);
            }
        }

        throw new DbUpdateConcurrencyException("Concurrent sell-item conflict after retries.");
    }

    private async Task<Transaction> TryHandle(SaleItemCommand request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .Include(x => x.Transactions)
            .Include(x => x.SaleHistory)
            .Include(x => x.InventoryItems)
                .ThenInclude(x => x.CaseItem)
            .FirstOrDefaultAsync(x => x.Id == usr.UserId, cancellationToken)
            ?? throw new NotFoundException("User not found");

        var item = user.InventoryItems.FirstOrDefault(x => x.Id == request.ItemId)
            ?? throw new NotFoundException("Item not found");

        user.SellItem(item);
        
        dbContext.InventoryItems.Remove(item);

        await dbContext.SaveChangesAsync(cancellationToken);

        return user.Transactions.Last();
    }
}