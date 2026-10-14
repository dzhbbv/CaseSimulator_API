using CaseSimulator.Domain.Entities;
using CaseSimulator.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using MediatR;

namespace CaseSimulator.Application.CaseItems;

public class GetCaseItemsQueryCommandHandler(
    IApplicationDbContext dbContext)
    : IRequestHandler<GetCaseItemsQueryCommand,
        IReadOnlyCollection<CaseItem>>
{
    public async Task<IReadOnlyCollection<CaseItem>> Handle(
        GetCaseItemsQueryCommand request,
        CancellationToken cancellationToken)
    {
        var caseItems = await dbContext.CaseItems
            .ToListAsync(cancellationToken);

        return caseItems.AsReadOnly();
    }
}