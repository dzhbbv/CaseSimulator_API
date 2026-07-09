using CaseSimulator.Domain.Common;
using CaseSimulator.Domain.ValueObjects;

namespace CaseSimulator.Domain.Entities;

public class CaseItem : BaseEntity
{
    public string Name { get; protected set; }
    public Rarity Rarity { get; protected set; }
    public Money Price { get; protected set; }
    public string ImageUrl { get; protected set; }

    private CaseItem() { }
    
    public CaseItem(string name, string imageUrl, Rarity rarity, Money price)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(imageUrl);
        Name = name.Trim().ToLowerInvariant();
        ImageUrl = imageUrl;
        Rarity = rarity;
        Price = price;
    }
}