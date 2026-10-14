using CaseSimulator.Domain.Entities;

namespace CaseSimulator.Application.Cases.Queries.GetCaseById;

public class CaseDetailDto
{
    public Guid Id { get; protected set; }
    public string Name { get; protected set; }
    public decimal Price { get; protected set; }
    public string ImageUrl { get; protected set; }
    public List<CaseContentDto> Items { get; protected set; }

    public CaseDetailDto(Case caseEntity)
    {
        Id = caseEntity.Id;
        Name = caseEntity.Name;
        Price = caseEntity.Price.Amount;
        ImageUrl = caseEntity.ImageUrl;
        Items = caseEntity.CaseContent.Select(c => new CaseContentDto(c)).ToList(); 
    }

    public class CaseContentDto
    {
        public Guid CaseItemId { get; protected set; }
        public string Name { get; protected set; }
        public decimal Price { get; protected set; }
        public string ImageUrl { get; protected set; }
        public string Rarity { get; protected set; }
        public decimal DropChance { get; protected set; }
        
        public CaseContentDto(CaseContent content)
        {
            CaseItemId = content.CaseItemId;
            Name = content.CaseItem.Name;
            Price = content.CaseItem.Price.Amount;
            ImageUrl = content.CaseItem.ImageUrl;
            Rarity =  content.CaseItem.Rarity.Name;
            DropChance = content.DropChance;
        }
    }
}