using CaseSimulator.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using CaseSimulator.Domain.Entities;

namespace CaseSimulator.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Case> Cases => Set<Case>();
    public DbSet<CaseItem> CaseItems => Set<CaseItem>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(builder);
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
}