using CaseSimulator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace CaseSimulator.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Case> Cases { get; }
    DbSet<CaseItem> CaseItems { get; }
    DbSet<InventoryItem> InventoryItems { get; }
    DbSet<Transaction> Transactions { get; }
    
    Task SaveChangesAsync(CancellationToken cancellationToken);
}