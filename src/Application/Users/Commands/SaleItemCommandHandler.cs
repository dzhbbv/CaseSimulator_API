using CaseSimulator.Application.Interfaces;
using CaseSimulator.Domain.Entities;
using CaseSimulator.Domain.Exception;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CaseSimulator.Application.Users.Commands;

public class SaleItemCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService usr ) : IRequestHandler<SaleItemCommand, Transaction>
{
    public async Task<Transaction> Handle(SaleItemCommand request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .Include(x => x.Transactions) 
            .Include(x => x.SaleHistory)
            .Include(x => x.InventoryItems)
            .ThenInclude(x => x.CaseItem)
            .FirstOrDefaultAsync(x => x.Id == usr.UserId, cancellationToken);
        
        if (user is null) throw new NotFoundException("User not found");

        var item = user.InventoryItems.FirstOrDefault(x => x.Id == request.ItemId);
        
        if (item is null) throw new NotFoundException("Item not found");
        
        user.SellItem(item);
        var newSaleHistory = user.SaleHistory.Last();
        var newTransaction = user.Transactions.Last(); 
        await dbContext.Transactions.AddAsync(newTransaction, cancellationToken);
        await dbContext.SaleItems.AddAsync(newSaleHistory, cancellationToken);
        
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return newTransaction;
    }
}