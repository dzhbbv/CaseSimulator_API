using CaseSimulator.Application.Interfaces;
using CaseSimulator.Domain.Entities;
using CaseSimulator.Domain.ValueObjects;
using MediatR;

namespace CaseSimulator.Application.Admin.Commands;

public class CreateCaseCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<CreateCaseCommand, Guid>
{
    public async Task<Guid> Handle(CreateCaseCommand request, CancellationToken cancellationToken)
    {
        var caseEntity = new Case(request.Name, request.ImageUrl, new Money(request.Price));
        await dbContext.Cases.AddAsync(caseEntity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return caseEntity.Id;
    }
}