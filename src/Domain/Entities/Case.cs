using CaseSimulator.Domain.Common;
using CaseSimulator.Domain.ValueObjects;

namespace CaseSimulator.Domain.Entities;

public class Case : BaseEntity
{
    public string Name { get; protected set; }
    public Money Price { get; protected set; }
    public string ImageUrl { get; protected set; }
    private List<CaseContent> _caseContent = new();
    public IReadOnlyCollection<CaseContent> CaseContent => _caseContent.AsReadOnly();

    public Case(string name, string imageUrl, Money price)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(imageUrl);
        Name = name;
        ImageUrl = imageUrl;
        Price = price;
    }
    
    public void AddItem(CaseItem item, decimal dropChance)
    {
        if (_caseContent.Any(c => c.CaseItemId == item.Id))
            throw new System.Exception("Item is already present");
        _caseContent.Add(new CaseContent(Id, item.Id, item, dropChance));
    }
}