using CaseSimulator.Domain.Entities;

namespace CaseSimulator.Application.Cases.Queries.GetCaseById;

public class CaseDetailDto
{
    public Guid Id { get; protected set; }
    public string Name { get; protected set; }
    public decimal Price { get; protected set; }
    public string ImageUrl { get; protected set; }
    public List<CaseItemDto> Items { get; protected set; }

    public CaseDetailDto(Case caseEntity)
    {
        Id = caseEntity.Id;
        Name = caseEntity.Name;
        Price = caseEntity.Price.Amount;
        ImageUrl = caseEntity.ImageUrl;
        Items = caseEntity.CaseContent.Select(c => new CaseItemDto(c.CaseItem)).ToList(); 
    }

    public class CaseItemDto
    {
        public Guid Id { get; protected set; }
        public string Name { get; protected set; }
        public decimal Price { get; protected set; }
        public string ImageUrl { get; protected set; }
        public string Rarity { get; protected set; }
        
        public CaseItemDto(CaseItem caseItem)
        {
            Id = caseItem.Id;
            Name = caseItem.Name;
            Price = caseItem.Price.Amount;
            ImageUrl = caseItem.ImageUrl;
            Rarity =  caseItem.Rarity.Name;
        }
    }
}