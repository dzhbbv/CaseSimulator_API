using CaseSimulator.Application.Common.Models;
using CaseSimulator.Application.Interfaces;
using CaseSimulator.Domain.Entities;
using CaseSimulator.Domain.Exception;
using CaseSimulator.Domain.Services;

namespace CaseSimulator.Infrastructure.Services;

public class CaseOpeningService : ICaseOpeningService
{
    public OpenCaseResult OpenCase(Case caseEntity, User user)
    {
        if (!caseEntity.IsConfiguredCorrectly())
            throw new InvalidCaseConfigurationException();

        var serverSeed = user.CurrentServerSeed;
        var serverSeedHash = user.CurrentServerSeedHash;
        var clientSeed = user.ClientSeed;
        var nonce = user.CurrentNonce;

        // Fail closed if the stored seed no longer matches the published commitment.
        if (!CaseSimulator.Domain.Services.ProvablyFairAlgorithm.IsCommitmentValid(serverSeed, serverSeedHash))
            throw new InvalidCaseConfigurationException();

        var entries = caseEntity.CaseContent
            .Select(x => new CaseSimulator.Domain.Services.ProvablyFairAlgorithm.DropEntry(x.CaseItemId, x.DropChance))
            .ToArray();
        var dropTableJson = CaseSimulator.Domain.Services.ProvablyFairAlgorithm.SerializeDropTable(entries);
        var winningItemId = CaseSimulator.Domain.Services.ProvablyFairAlgorithm.SelectItem(serverSeed, clientSeed, nonce, entries);
        var winningItem = caseEntity.CaseContent.Single(x => x.CaseItemId == winningItemId).CaseItem;

        var round = new ProvablyFairRound(
            user.Id,
            caseEntity.Id,
            winningItem.Id,
            serverSeed,
            serverSeedHash,
            clientSeed,
            nonce,
            dropTableJson,
            CaseSimulator.Domain.Services.ProvablyFairAlgorithm.Version);

        user.IncrementNonce();
        return new OpenCaseResult(winningItem, round);
    }
}
