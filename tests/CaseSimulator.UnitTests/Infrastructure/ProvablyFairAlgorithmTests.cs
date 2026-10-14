using CaseSimulator.Domain.Services;
using FluentAssertions;
using Xunit;

namespace CaseSimulator.UnitTests.Infrastructure;

public class ProvablyFairAlgorithmTests
{
    [Fact]
    public void Commitment_ShouldBeStableAndVerifiable()
    {
        const string seed = "0123456789ABCDEF";
        var hash = ProvablyFairAlgorithm.ComputeServerSeedHash(seed);

        hash.Should().HaveLength(64);
        ProvablyFairAlgorithm.IsCommitmentValid(seed, hash).Should().BeTrue();
        ProvablyFairAlgorithm.IsCommitmentValid(seed + "x", hash).Should().BeFalse();
    }

    [Fact]
    public void Roll_ShouldBeDeterministicAndInRange()
    {
        var first = ProvablyFairAlgorithm.ComputeRoll("server-seed", "client-seed", 12);
        var second = ProvablyFairAlgorithm.ComputeRoll("server-seed", "client-seed", 12);

        first.Should().Be(second);
        first.Should().BeGreaterThanOrEqualTo(0m).And.BeLessThan(1m);
    }
    
    [Fact]
    public void VerifyRound_ShouldVerifyResultAndRejectTampering()
    {
        const string seed = "server-seed";
        var hash = ProvablyFairAlgorithm.ComputeServerSeedHash(seed);

        var table = new[]
        {
            new ProvablyFairAlgorithm.DropEntry(
                Guid.Parse("00000000-0000-0000-0000-000000000001"), 0.4m),
            new ProvablyFairAlgorithm.DropEntry(
                Guid.Parse("00000000-0000-0000-0000-000000000002"), 0.6m)
        };

        var json = ProvablyFairAlgorithm.SerializeDropTable(table);
        var expected = ProvablyFairAlgorithm.SelectItem(
            seed, "client-seed", 7, table);

        ProvablyFairAlgorithm.VerifyRound(
                seed, hash, "client-seed", 7, json, expected)
            .Should().BeTrue();

        var wrongExpected = table
            .Select(x => x.CaseItemId)
            .First(id => id != expected);

        ProvablyFairAlgorithm.VerifyRound(
                seed, hash, "client-seed", 7, json, wrongExpected)
            .Should().BeFalse();

        ProvablyFairAlgorithm.VerifyRound(
                seed, "wrong-hash", "client-seed", 7, json, expected)
            .Should().BeFalse();
    }

    [Fact]
    public void SerializeDropTable_ShouldBeIndependentOfInputOrder()
    {
        var firstId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var secondId = Guid.Parse("00000000-0000-0000-0000-000000000002");
        var first = ProvablyFairAlgorithm.SerializeDropTable(new[]
        {
            new ProvablyFairAlgorithm.DropEntry(secondId, 0.5m),
            new ProvablyFairAlgorithm.DropEntry(firstId, 0.5m)
        });
        var second = ProvablyFairAlgorithm.SerializeDropTable(new[]
        {
            new ProvablyFairAlgorithm.DropEntry(firstId, 0.5m),
            new ProvablyFairAlgorithm.DropEntry(secondId, 0.5m)
        });

        first.Should().Be(second);
    }
}
