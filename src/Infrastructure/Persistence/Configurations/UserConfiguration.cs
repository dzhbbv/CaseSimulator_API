using CaseSimulator.Domain.Entities;
using CaseSimulator.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CaseSimulator.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Username).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Email).IsRequired().HasMaxLength(50);

        builder.Property(x => x.Balance)
            .HasConversion<decimal>(
                balance => balance.Amount,
                dbValue => new Money(dbValue));
        
        builder.HasMany(x => x.InventoryItems).WithOne().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Transactions).WithOne().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(x => x.Username).IsUnique();
        builder.HasIndex(x => x.Email).IsUnique();
    }
}