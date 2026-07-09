using CaseSimulator.Domain.Common;
using CaseSimulator.Domain.Exception;

namespace CaseSimulator.Domain.Entities;

public class CaseContent : BaseEntity
{
    public Guid CaseId { get; protected set; }    
    public Guid CaseItemId { get; protected set; }
    public CaseItem CaseItem { get; protected set; }
    public decimal DropChance { get; protected set; }

    private CaseContent() { }
    
    public CaseContent(Guid caseId, Guid itemId, CaseItem caseItem, decimal dropChance)
    {
        if (dropChance <= 0 || dropChance > 1)
            throw new InvalidCaseConfigurationException();
            
        CaseId = caseId;
        CaseItemId = itemId;
        CaseItem = caseItem;
        DropChance = dropChance;
    }
}