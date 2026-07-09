using CaseSimulator.Domain.Common;
using CaseSimulator.Domain.ValueObjects;

namespace CaseSimulator.Domain.Entities;

public class SaleItem : BaseEntity
{
    public Guid UserId { get; protected set; }
    public Guid CaseItemId { get; protected set; }
    public Money Price { get; protected set; }
    public DateTime SoldDate { get; protected set; } =  DateTime.UtcNow;

    private SaleItem() { }
    
    public SaleItem(Guid userId, Guid caseItemId, Money price)
    {
        UserId = userId;
        CaseItemId = caseItemId;
        Price = price;
    }
}