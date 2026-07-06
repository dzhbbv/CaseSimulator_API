using CaseSimulator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CaseSimulator.Infrastructure.Persistence.Configurations;

public class CaseContentConfiguration : IEntityTypeConfiguration<CaseContent>
{
    public void Configure(EntityTypeBuilder<CaseContent> builder)
    {
        builder.ToTable("CaseContents");
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.DropChance).IsRequired();
        
        builder.HasIndex(x => new { x.CaseId, x.CaseItemId }).IsUnique();

        builder.HasOne(x => x.CaseItem)
            .WithMany()
            .HasForeignKey(x => x.CaseItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}