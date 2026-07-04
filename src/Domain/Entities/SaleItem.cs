using CaseSimulator.Domain.ValueObjects;

namespace CaseSimulator.Domain.Entities;

public class SaleItem
{
    public Guid UserId { get; protected set; }
    public Guid CaseItemId { get; protected set; }
    public Money Price { get; protected set; }
    public DateTime SoldDate { get; protected set; } =  DateTime.UtcNow;

    public SaleItem(Guid userId, Guid caseItemId, Money price)
    {
        UserId = userId;
        CaseItemId = caseItemId;
        Price = price;
    }
}