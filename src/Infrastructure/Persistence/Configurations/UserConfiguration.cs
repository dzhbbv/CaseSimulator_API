using CaseSimulator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CaseSimulator.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Username)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.PasswordHash)
            .IsRequired();

        builder.Property(x => x.ClientSeed)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.CurrentServerSeed)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.CurrentServerSeedHash)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.CurrentNonce)
            .IsRequired();

        builder.OwnsOne(x => x.Balance, balance =>
        {
            balance.Property(b => b.Amount)
                .HasColumnName("Balance")
                .IsRequired();
        });

        builder.HasMany(x => x.InventoryItems)
            .WithOne()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Transactions)
            .WithOne()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.SaleHistory)
            .WithOne()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.Username)
            .IsUnique();

        builder.HasIndex(x => x.Email)
            .IsUnique();
    }
}