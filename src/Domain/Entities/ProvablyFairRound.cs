using CaseSimulator.Domain.Common;

namespace CaseSimulator.Domain.Entities;

public class ProvablyFairRound : BaseEntity
{
    public Guid UserId { get; protected set; }
    public Guid CaseId { get; protected set; }
    public Guid CaseItemId { get; protected set; }
    /// <summary>Stored privately and returned by the API only after this seed is rotated.</summary>
    public string ServerSeed { get; protected set; } = string.Empty;
    public string ServerSeedHash { get; protected set; } = string.Empty;
    public string ClientSeed { get; protected set; } = string.Empty;
    public int Nonce { get; protected set; }
    /// <summary>Immutable JSON snapshot of item IDs and probabilities at opening time.</summary>
    public string DropTableJson { get; protected set; } = "[]";
    public string Algorithm { get; protected set; } = "legacy-sha256-v0";

    private ProvablyFairRound() { }

    public ProvablyFairRound(
        Guid userId,
        Guid caseId,
        Guid caseItemId,
        string serverSeed,
        string serverSeedHash,
        string clientSeed,
        int nonce,
        string dropTableJson = "[]",
        string algorithm = "legacy-sha256-v0")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(serverSeed);
        ArgumentException.ThrowIfNullOrWhiteSpace(serverSeedHash);
        ArgumentException.ThrowIfNullOrWhiteSpace(clientSeed);
        ArgumentException.ThrowIfNullOrWhiteSpace(dropTableJson);
        ArgumentException.ThrowIfNullOrWhiteSpace(algorithm);
        if (nonce < 0) throw new ArgumentOutOfRangeException(nameof(nonce));

        UserId = userId;
        CaseId = caseId;
        CaseItemId = caseItemId;
        ServerSeed = serverSeed;
        ServerSeedHash = serverSeedHash;
        ClientSeed = clientSeed;
        Nonce = nonce;
        DropTableJson = dropTableJson;
        Algorithm = algorithm;
    }
}
