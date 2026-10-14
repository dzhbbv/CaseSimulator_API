using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace CaseSimulator.Domain.Services;

/// <summary>
/// Versioned Provably Fair algorithm.
/// Commitment: SHA256(UTF8(serverSeed)), uppercase hex.
/// Random: HMAC-SHA256(key=UTF8(serverSeed), data=UTF8($"{clientSeed}:{nonce}"));
/// first 13 hexadecimal characters interpreted as an unsigned integer, divided by 2^52.
/// The drop table is sorted by CaseItemId in ordinal GUID-N format before selection.
/// </summary>
public static class ProvablyFairAlgorithm
{
    public const string Version = "hmac-sha256-52bit-v1";
    private const ulong RandomRange = 1UL << 52;

    public sealed record DropEntry(Guid CaseItemId, decimal DropChance);

    public static string GenerateServerSeed() => Convert.ToHexString(RandomNumberGenerator.GetBytes(32));

    public static string ComputeServerSeedHash(string serverSeed)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(serverSeed);
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(serverSeed)));
    }

    public static bool IsCommitmentValid(string serverSeed, string expectedHash)
    {
        if (string.IsNullOrWhiteSpace(serverSeed) || string.IsNullOrWhiteSpace(expectedHash))
            return false;

        try
        {
            var actual = Convert.FromHexString(ComputeServerSeedHash(serverSeed));
            var expected = Convert.FromHexString(expectedHash);
            return expected.Length == actual.Length && CryptographicOperations.FixedTimeEquals(actual, expected);
        }
        catch (FormatException)
        {
            return false;
        }
    }

    public static string SerializeDropTable(IEnumerable<DropEntry> entries)
    {
        var ordered = entries.OrderBy(x => x.CaseItemId.ToString("N"), StringComparer.Ordinal).ToArray();
        ValidateDropTable(ordered);
        return JsonSerializer.Serialize(ordered);
    }

    public static decimal ComputeRoll(string serverSeed, string clientSeed, int nonce)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(serverSeed);
        ArgumentException.ThrowIfNullOrWhiteSpace(clientSeed);
        if (nonce < 0) throw new ArgumentOutOfRangeException(nameof(nonce));

        var key = Encoding.UTF8.GetBytes(serverSeed);
        var message = Encoding.UTF8.GetBytes($"{clientSeed}:{nonce.ToString(CultureInfo.InvariantCulture)}");
        var digestHex = Convert.ToHexString(HMACSHA256.HashData(key, message));
        var first52Bits = ulong.Parse(digestHex[..13], NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        return (decimal)first52Bits / RandomRange;
    }

    public static Guid SelectItem(string serverSeed, string clientSeed, int nonce, IReadOnlyList<DropEntry> entries)
    {
        var ordered = entries.OrderBy(x => x.CaseItemId.ToString("N"), StringComparer.Ordinal).ToArray();
        ValidateDropTable(ordered);
        var roll = ComputeRoll(serverSeed, clientSeed, nonce);
        decimal cumulative = 0m;

        foreach (var entry in ordered)
        {
            cumulative += entry.DropChance;
            if (roll < cumulative)
                return entry.CaseItemId;
        }

        // The roll is always < 1 and validated probabilities sum to exactly 1.
        throw new InvalidOperationException("The roll did not map to an item. The drop table is invalid.");
    }

    public static bool VerifyRound(
        string serverSeed,
        string serverSeedHash,
        string clientSeed,
        int nonce,
        string dropTableJson,
        Guid expectedCaseItemId)
    {
        if (!IsCommitmentValid(serverSeed, serverSeedHash)) return false;
        if (string.IsNullOrWhiteSpace(dropTableJson)) return false;

        try
        {
            var entries = JsonSerializer.Deserialize<List<DropEntry>>(dropTableJson);
            if (entries is null || entries.Count == 0) return false;
            return SelectItem(serverSeed, clientSeed, nonce, entries) == expectedCaseItemId;
        }
        catch (global::System.Exception ex) when (ex is JsonException or ArgumentException or InvalidOperationException or OverflowException)
        {
            return false;
        }
    }

    private static void ValidateDropTable(IReadOnlyCollection<DropEntry> entries)
    {
        if (entries.Count == 0 || entries.Any(x => x.CaseItemId == Guid.Empty || x.DropChance <= 0m || x.DropChance > 1m))
            throw new ArgumentException("Drop table must contain item IDs and probabilities between 0 and 1.", nameof(entries));
        if (entries.Select(x => x.CaseItemId).Distinct().Count() != entries.Count)
            throw new ArgumentException("Drop table cannot contain duplicate item IDs.", nameof(entries));
        if (entries.Sum(x => x.DropChance) != 1m)
            throw new ArgumentException("Drop probabilities must sum to exactly 1.", nameof(entries));
    }
}
