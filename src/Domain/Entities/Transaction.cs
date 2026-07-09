using CaseSimulator.Domain.Common;
using CaseSimulator.Domain.Enums;
using CaseSimulator.Domain.ValueObjects;

namespace CaseSimulator.Domain.Entities;

public class Transaction : BaseEntity
{
    public Guid UserId { get; protected set; }
    public Money CurrentBalance { get; protected set; }
    public Money Amount { get; protected set; }
    public TransactionType Type { get; protected set; }
    public Money BalanceAfter { get; protected set; }

    private Transaction() { }
    
    public Transaction(Guid userId, Money balance, Money amount, TransactionType type)
    {
        UserId = userId;
        Amount = amount;
        Type = type;
        CurrentBalance = balance;
        BalanceAfter = type switch
        {
            TransactionType.Deposit => balance + amount,
            TransactionType.Sale => balance + amount,
            TransactionType.CaseOpen => balance - amount,
            TransactionType.Withdraw => balance - amount,
            _ => throw new ArgumentException("Unknown transaction type")
        };
    }
}