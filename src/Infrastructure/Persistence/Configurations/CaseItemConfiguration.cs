using CaseSimulator.Domain.Entities;
using CaseSimulator.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CaseSimulator.Infrastructure.Persistence.Configurations;

public class CaseItemConfiguration : IEntityTypeConfiguration<CaseItem>
{
    public void Configure(EntityTypeBuilder<CaseItem> builder)
    {
        builder.ToTable("CaseItems");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Name).HasMaxLength(64).IsRequired();
        builder.Property(x => x.ImageUrl).HasMaxLength(256).IsRequired();
        
        builder.OwnsOne(x => x.Price, price =>
        {
            price.Property(p => p.Amount)
                .HasColumnName("Price")
                .IsRequired();
        });
        
        builder.Property(x => x.Rarity)
            .HasConversion<string>(
                x => x.Name,
                dbValue => Rarity.FromName(dbValue));
        
        builder.HasIndex(x => x.Name).IsUnique();
    }
}