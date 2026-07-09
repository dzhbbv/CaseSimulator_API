using CaseSimulator.Application.Interfaces;
using CaseSimulator.Domain.Entities;
using CaseSimulator.Domain.ValueObjects;
using CaseSimulator.Domain.Exception;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CaseSimulator.Application.Admin.Commands;

public class CreateCaseCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<CreateCaseCommand, Guid>
{
    public async Task<Guid> Handle(CreateCaseCommand request, CancellationToken cancellationToken)
    {
        var normalizedName = request.Name.Trim().ToLowerInvariant();

        var exists = await dbContext.Cases
            .AnyAsync(c => c.Name == normalizedName, cancellationToken);

        if (exists)
            throw new AlreadyExistingException($"Case with name '{request.Name}' already exists.");

        var caseEntity = new Case(request.Name, request.ImageUrl, new Money(request.Price));
        
        await dbContext.Cases.AddAsync(caseEntity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return caseEntity.Id;
    }
}