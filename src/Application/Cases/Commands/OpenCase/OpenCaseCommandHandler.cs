using CaseSimulator.Application.Interfaces;
using CaseSimulator.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CaseSimulator.Application.Cases.Commands.OpenCase;

public class OpenCaseCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService,
    ICaseOpeningService caseOpeningService) : IRequestHandler<OpenCaseCommand, Guid>
{
    public async Task<Guid> Handle(OpenCaseCommand request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .Include(u => u.InventoryItems)
            .Include(u => u.Transactions)
            .FirstOrDefaultAsync(u => u.Id == currentUserService.UserId, cancellationToken);
        
        var caseEntity = await dbContext.Cases
            .Include(c => c.CaseContent)
            .ThenInclude(cc => cc.CaseItem)
            .FirstOrDefaultAsync(c => c.Id == request.CaseId, cancellationToken);

        if (user is null) throw new Exception("User not found");
        if (caseEntity is null) throw new Exception("Case not found");

        user.SpendOnCase(caseEntity.Price);

        var result = caseOpeningService.OpenCase(caseEntity, user);

        var newInventoryItem = new InventoryItem(user.Id, result.CaseItem.Id, result.CaseItem);
        user.AddInventoryItem(newInventoryItem);

        var newTransaction = user.Transactions.Last(); 
        
        await dbContext.ProvablyFairRounds.AddAsync(result.Round, cancellationToken);
        await dbContext.InventoryItems.AddAsync(newInventoryItem, cancellationToken);
        await dbContext.Transactions.AddAsync(newTransaction, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return result.CaseItem.Id;
    }
}