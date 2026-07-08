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
            .FirstOrDefaultAsync(u => u.Id == currentUserService.UserId, cancellationToken);
        
        var caseEntity = await dbContext.Cases
            .Include(c => c.CaseContent)
            .ThenInclude(cc => cc.CaseItem)
            .FirstOrDefaultAsync(c => c.Id == request.CaseId, cancellationToken);

        if (user is null)
            throw new Exception("User not found");
        if (caseEntity is null)
            throw new Exception("Case not found");

        user.SpendOnCase(caseEntity.Price);

        var result = caseOpeningService.OpenCase(caseEntity, user);

        user.AddInventoryItem(new InventoryItem(user.Id, result.CaseItem.Id, result.CaseItem));

        await dbContext.ProvablyFairRounds.AddAsync(result.Round, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return result.CaseItem.Id;
    }
}