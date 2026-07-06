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
            .FirstOrDefaultAsync(c => c.Id == request.CaseId, cancellationToken);
        if (user is null)
            throw new Exception("User not found");
        if (caseEntity is null)
            throw new Exception("Case not found");
        user.SpendOnCase(caseEntity.Price);
        var caseItem = caseOpeningService.OpenCase(caseEntity);
        user.AddInventoryItem(new InventoryItem(user.Id, caseItem.Id, caseItem));
        await dbContext.SaveChangesAsync();
        return caseItem.Id;
    }
}