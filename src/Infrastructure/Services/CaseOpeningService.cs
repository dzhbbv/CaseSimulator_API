using CaseSimulator.Application.Interfaces;
using CaseSimulator.Domain.Entities;
using System.Security.Cryptography;
using System.Text;

namespace CaseSimulator.Infrastructure.Services;

public class CaseOpeningService : ICaseOpeningService
{
    public CaseItem OpenCase(Case caseEntity, string clientSeed)
    {
        var serverSeed = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(serverSeed));
        var serverSeedHash = Convert.ToHexString(hashBytes);
        
        var items = caseEntity.CaseContent;
        
        var input = $"{serverSeed}:{clientSeed}:{nonce}";
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        ulong value = BitConverter.ToUInt64(bytes, 0);
        
        decimal random = (decimal)value / ulong.MaxValue;
        decimal cumulative = 0;

        foreach (var itm in items)
        {
            cumulative += itm.DropChance;
            if (cumulative >= random)
                return itm.CaseItem;
        } 
        throw new Exception("Error");
    }
}