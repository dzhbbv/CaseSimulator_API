using CaseSimulator.Domain.Common;

namespace CaseSimulator.Domain.Entities;

public class CaseContent : BaseEntity
{
    public Guid CaseId { get; protected set; }    
    public Guid CaseItemId { get; protected set; }
    public CaseItem CaseItem { get; protected set; }
    public decimal DropChance { get; protected set; }

    public CaseContent(Guid caseId, Guid itemId, CaseItem caseItem, decimal dropChance)
    {
        if (dropChance <= 0 || dropChance > 1)
            throw new ArgumentException("DropChance must be between 0 and 1");
        CaseId = caseId;
        CaseItemId = itemId;
        CaseItem = caseItem;
        DropChance = dropChance;
    }
}