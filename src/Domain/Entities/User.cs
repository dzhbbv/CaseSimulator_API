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
    
    public string ClientSeed { get; protected set; }
    public string CurrentServerSeed { get; protected set; }
    public string CurrentServerSeedHash { get; protected set; }
    public int CurrentNonce { get; protected set; } = 0;
    
    private List<Transaction> _transactions = new();
    private List<InventoryItem> _inventoryItems = new();
    private List<SaleItem> _saleHistory = new();
    public IReadOnlyCollection<Transaction> Transactions => _transactions.AsReadOnly();
    public IReadOnlyCollection<InventoryItem> InventoryItems => _inventoryItems.AsReadOnly();
    public IReadOnlyCollection<SaleItem> SaleHistory => _saleHistory.AsReadOnly();

    public void IncrementNonce() => CurrentNonce++;
    
    public User(string username, string passwordHash, string email, string clientSeed, string serverSeed, string serverSeedHash)
    {
        if (string.IsNullOrWhiteSpace(username)) throw new ArgumentNullException(nameof(username));
        if (string.IsNullOrWhiteSpace(passwordHash)) throw new ArgumentNullException(nameof(passwordHash));
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentNullException(nameof(email));
        if (string.IsNullOrWhiteSpace(clientSeed)) throw new ArgumentNullException(nameof(clientSeed));
        if (string.IsNullOrWhiteSpace(serverSeedHash)) throw new ArgumentNullException(nameof(serverSeedHash));
        if (string.IsNullOrWhiteSpace(serverSeed)) throw new ArgumentNullException(nameof(serverSeed));
        Username = username.Trim().ToLowerInvariant();
        PasswordHash = passwordHash;
        Email = email.Trim().ToLowerInvariant();
        ClientSeed = clientSeed;
        CurrentServerSeed = serverSeed;
        CurrentServerSeedHash = serverSeedHash;
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
        _saleHistory.Add(new SaleItem(Id, item.CaseItem.Id, item.CaseItem.Price));
        _transactions.Add(new Transaction(Id, Balance, item.CaseItem.Price, TransactionType.Sale));
        Balance += item.CaseItem.Price;
        _inventoryItems.Remove(item);
    }
    
    public void AddInventoryItem(InventoryItem inventoryItem)
    {
        _inventoryItems.Add(inventoryItem);
    }
}