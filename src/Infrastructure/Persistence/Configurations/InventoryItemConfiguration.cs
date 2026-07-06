using CaseSimulator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CaseSimulator.Infrastructure.Persistence.Configurations;

public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        builder.ToTable("InventoryItems");
        
        builder.HasKey(x => x.Id);
        
        builder.HasIndex(x => x.UserId);

        builder.HasOne(x => x.CaseItem)
            .WithMany()
            .HasForeignKey(x => x.CaseItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}