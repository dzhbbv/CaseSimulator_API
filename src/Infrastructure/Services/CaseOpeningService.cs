using System.Security.Cryptography;
using System.Text;
using CaseSimulator.Application.Common.Models;
using CaseSimulator.Application.Interfaces;
using CaseSimulator.Domain.Entities;
using CaseSimulator.Domain.Exception;

namespace CaseSimulator.Infrastructure.Services;

public class CaseOpeningService : ICaseOpeningService
{
    public OpenCaseResult OpenCase(Case caseEntity, User user)
    {
        if (!caseEntity.IsConfiguredCorrectly())
            throw new InvalidCaseConfigurationException();

        var serverSeed = user.CurrentServerSeed;
        var clientSeed = user.ClientSeed;
        var nonce = user.CurrentNonce;
        
        var input = $"{serverSeed}:{clientSeed}:{nonce}";
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        ulong value = BitConverter.ToUInt64(bytes, 0);
        decimal random = (decimal)value / ulong.MaxValue;
        
        decimal cumulative = 0;
        foreach (var item in caseEntity.CaseContent)
        {
            cumulative += item.DropChance;
            if (random <= cumulative)
            {
                var round = new ProvablyFairRound(
                    user.Id,
                    caseEntity.Id,
                    item.CaseItem.Id,
                    serverSeed,
                    user.CurrentServerSeedHash,
                    clientSeed,
                    nonce);

                user.IncrementNonce();

                return new OpenCaseResult(item.CaseItem, round);
            }
        }
        
        throw new InvalidCaseConfigurationException();
    }
}