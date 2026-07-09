using CaseSimulator.Domain.Common;

namespace CaseSimulator.Domain.Entities;

public class InventoryItem : BaseEntity
{
    public Guid UserId { get; protected set; }
    public Guid CaseItemId { get; protected set; }
    public CaseItem CaseItem { get; protected set; }

    private InventoryItem() { }
    
    public InventoryItem(Guid userId, Guid caseItemId, CaseItem caseItem)
    {
        UserId = userId;
        CaseItemId = caseItemId;
        CaseItem = caseItem;
    }
}