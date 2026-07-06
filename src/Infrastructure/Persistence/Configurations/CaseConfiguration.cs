using CaseSimulator.Domain.Entities;
using CaseSimulator.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CaseSimulator.Infrastructure.Persistence.Configurations;

public class CaseConfiguration : IEntityTypeConfiguration<Case>
{
    public void Configure(EntityTypeBuilder<Case> builder)
    {
        builder.ToTable("Cases");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Name).HasMaxLength(64).IsRequired();
        builder.Property(x=>x.ImageUrl).HasMaxLength(256).IsRequired();
        
        builder.Property(x => x.Price)
            .HasConversion<decimal>(
                price => price.Amount,
                dbValue => new Money(dbValue));
        
        builder.HasMany(x => x.CaseContent).WithOne()
            .HasForeignKey(x => x.CaseId).OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(x => x.Name).IsUnique();
    }
}