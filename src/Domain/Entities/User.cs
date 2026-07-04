using CaseSimulator.Domain.Common;
using CaseSimulator.Domain.Enums;
using CaseSimulator.Domain.Exception;
using CaseSimulator.Domain.ValueObjects;

namespace CaseSimulator.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; protected set; }
    public string PasswordHash { get; protected set; }
    public string Email { get; protected set; }
    public Money Balance { get; protected set; } = Money.Zero;
    private List<Transaction> _transactions = new();
    private List<InventoryItem> _inventoryItems = new();
    private List<SaleItem> _saleHistory = new();
    public IReadOnlyCollection<Transaction> Transactions => _transactions.AsReadOnly();
    public IReadOnlyCollection<InventoryItem> InventoryItems => _inventoryItems.AsReadOnly();
    public IReadOnlyCollection<SaleItem> SaleHistory => _saleHistory.AsReadOnly();

    public User(string username, string passwordHash, string email)
    {
        if (string.IsNullOrWhiteSpace(username)) throw new ArgumentNullException(nameof(username));
        if (string.IsNullOrWhiteSpace(passwordHash)) throw new ArgumentNullException(nameof(passwordHash));
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentNullException(nameof(email));
        Username = username;
        PasswordHash = passwordHash;
        Email = email;
    }

    public void Deposit(Money amount)
    {
        _transactions.Add(new Transaction(Id, Balance, amount, TransactionType.Deposit));
        Balance += amount;
    }

    public void Withdraw(Money amount)
    {
        if (Balance < amount)
            throw new InsufficientBalanceException(Balance.Amount, amount.Amount);
        _transactions.Add(new Transaction(Id, Balance, amount, TransactionType.Withdraw));
        Balance -= amount;
    }

    public void SpendOnCase(Money amount)
    {
        if (Balance < amount)
            throw new InsufficientBalanceException(Balance.Amount, amount.Amount);
        _transactions.Add(new Transaction(Id, Balance, amount, TransactionType.CaseOpen));
        Balance -= amount;
    }

    public void SellItem(InventoryItem item)
    {
        _saleHistory.Add(new SaleRecord(Id, item.CaseItem.Id, item.CaseItem.Price));
        _transactions.Add(new Transaction(Id, Balance, item.CaseItem.Price, TransactionType.Sale));
        Balance += item.CaseItem.Price;
        _inventoryItems.Remove(item);
    }
    
    public void AddInventoryItem(InventoryItem inventoryItem)
    {
        _inventoryItems.Add(inventoryItem);
    }
}