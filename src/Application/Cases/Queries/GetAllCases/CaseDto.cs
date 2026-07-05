using CaseSimulator.Domain.Entities;

namespace CaseSimulator.Application.Cases.Queries.GetAllCases;

public class CaseDto
{
    public Guid Id { get; protected set; }
    public string Name { get; protected set; }
    public decimal Price { get; protected set; }
    public string ImageUrl { get; protected set; }

    public CaseDto(Case caseEntity)
    {
        Id = caseEntity.Id;
        Name = caseEntity.Name;
        Price = caseEntity.Price.Amount;
        ImageUrl = caseEntity.ImageUrl;
    }
}