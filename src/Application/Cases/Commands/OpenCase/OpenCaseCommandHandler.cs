using CaseSimulator.Application.Interfaces;
using CaseSimulator.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CaseSimulator.Application.Cases.Commands.OpenCase;

public class OpenCaseCommandHandler(IApplicationDbContext dbContext, ICurrentUserService usr, ICaseOpeningService caseOpeningService) : IRequestHandler<OpenCaseCommand, Guid>
{
    public async Task<Guid> Handle(OpenCaseCommand request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == usr.UserId, cancellationToken);
        var caseEntity = await dbContext.Cases
            .Include(c => c.CaseContent)
            .ThenInclude(cc => cc.CaseItem)
            .FirstOrDefaultAsync(c => c.Id == request.CaseId, cancellationToken);
        if (user is null)
            throw new KeyNotFoundException("User not found.");
        if (caseEntity is null)
            throw new KeyNotFoundException("Case not found.");
        user.SpendOnCase(caseEntity.Price);
        var result = caseOpeningService.OpenCase(caseEntity, user);
        user.AddInventoryItem(new InventoryItem(user.Id, result.CaseItem.Id, result.CaseItem));
        dbContext.ProvablyFairRounds.Add(result.Round);
        await dbContext.SaveChangesAsync(cancellationToken);
        return result.CaseItem.Id;
    }
}