using CaseSimulator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CaseSimulator.Infrastructure.Persistence.Configurations;

public class ProvablyFairRoundConfiguration : IEntityTypeConfiguration<ProvablyFairRound>
{
    public void Configure(EntityTypeBuilder<ProvablyFairRound> builder)
    {
        builder.ToTable("ProvablyFairRounds");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ServerSeed)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.ServerSeedHash)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(x => x.ClientSeed)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.Nonce)
            .IsRequired();

        builder.HasIndex(x => x.UserId);

        builder.HasIndex(x => x.CaseId);

        builder.HasIndex(x => x.ServerSeedHash);
    }
}