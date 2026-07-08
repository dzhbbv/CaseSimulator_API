using CaseSimulator.Domain.Common;

namespace CaseSimulator.Domain.Entities;

public class ProvablyFairRound : BaseEntity
{
    public Guid UserId { get; protected set; }
    public Guid CaseId { get; protected set; }
    public Guid CaseItemId { get; protected set; }

    public string ServerSeed { get; protected set; }
    public string ServerSeedHash { get; protected set; }
    public string ClientSeed { get; protected set; }

    public int Nonce { get; protected set; }

    private ProvablyFairRound() { }

    public ProvablyFairRound(
        Guid userId,
        Guid caseId,
        Guid caseItemId,
        string serverSeed,
        string serverSeedHash,
        string clientSeed,
        int nonce)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(serverSeed);
        ArgumentException.ThrowIfNullOrWhiteSpace(serverSeedHash);
        ArgumentException.ThrowIfNullOrWhiteSpace(clientSeed);

        UserId = userId;
        CaseId = caseId;
        CaseItemId = caseItemId;

        ServerSeed = serverSeed;
        ServerSeedHash = serverSeedHash;
        ClientSeed = clientSeed;

        Nonce = nonce;
    }
}