using CaseSimulator.Application.Interfaces;
using CaseSimulator.Domain.Entities;
using CaseSimulator.Domain.Exception;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CaseSimulator.Application.Cases.Commands.OpenCase;

public class OpenCaseCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService,
    ICaseOpeningService caseOpeningService) : IRequestHandler<OpenCaseCommand, Guid>
{
    private const int MaxRetries = 3;

    public async Task<Guid> Handle(OpenCaseCommand request, CancellationToken cancellationToken)
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

        throw new DbUpdateConcurrencyException("Concurrent case-opening conflict after retries.");
    }

    private async Task<Guid> TryHandle(OpenCaseCommand request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .Include(u => u.InventoryItems)
            .Include(u => u.Transactions)
            .FirstOrDefaultAsync(u => u.Id == currentUserService.UserId, cancellationToken)
            ?? throw new NotFoundException("User not found");

        var caseEntity = await dbContext.Cases
            .Include(c => c.CaseContent)
            .ThenInclude(cc => cc.CaseItem)
            .FirstOrDefaultAsync(c => c.Id == request.CaseId, cancellationToken)
            ?? throw new NotFoundException("Case not found");

        if (!caseEntity.IsConfiguredCorrectly())
            throw new InvalidCaseConfigurationException();

        user.SpendOnCase(caseEntity.Price);

        var result = caseOpeningService.OpenCase(caseEntity, user);

        var newInventoryItem = new InventoryItem(user.Id, result.CaseItem.Id, result.CaseItem);
        user.AddInventoryItem(newInventoryItem);
        
        await dbContext.ProvablyFairRounds.AddAsync(result.Round, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return result.CaseItem.Id;
    }
}