using CaseSimulator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CaseSimulator.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Case> Cases { get; }
    DbSet<CaseItem> CaseItems { get; }
    DbSet<InventoryItem> InventoryItems { get; }
    DbSet<Transaction> Transactions { get; }
    DbSet<SaleItem> SaleItems { get; }
    DbSet<ProvablyFairRound> ProvablyFairRounds { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<CaseContent> CaseContents { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}