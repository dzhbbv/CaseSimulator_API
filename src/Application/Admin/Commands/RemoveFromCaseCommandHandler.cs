using CaseSimulator.Application.Interfaces;
using CaseSimulator.Domain.Exception;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CaseSimulator.Application.Admin.Commands;

public class RemoveItemFromCaseCommandHandler(
    IApplicationDbContext dbContext)
    : IRequestHandler<RemoveItemFromCaseCommand>
{
    public async Task Handle(
        RemoveItemFromCaseCommand request,
        CancellationToken cancellationToken)
    {
        var caseContent = await dbContext.CaseContents
            .FirstOrDefaultAsync(
                x => x.CaseId == request.CaseId
                     && x.CaseItemId == request.ItemId,
                cancellationToken);

        if (caseContent is null)
            throw new NotFoundException(
                "Предмет не найден в указанном кейсе.");

        dbContext.CaseContents.Remove(caseContent);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}