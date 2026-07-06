using CaseSimulator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CaseSimulator.Infrastructure.Persistence.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");
        
        builder.HasKey(x => x.Id);
        
        builder.HasIndex(x => x.UserId);
        
        builder.Property(x => x.Type)
            .HasConversion<string>();
        
        builder.OwnsOne(x => x.Amount, price =>
        {
            price.Property(p => p.Amount)
                .HasColumnName("Amount")
                .IsRequired();
        });
        builder.OwnsOne(x => x.CurrentBalance, price =>
        {
            price.Property(p => p.Amount)
                .HasColumnName("CurrentBalance")
                .IsRequired();
        });
        builder.OwnsOne(x => x.BalanceAfter, price =>
        {
            price.Property(p => p.Amount)
                .HasColumnName("BalanceAfter")
                .IsRequired();
        });
    }
}