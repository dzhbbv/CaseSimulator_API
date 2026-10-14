using System.Security.Cryptography;
using System.Text;

namespace CaseSimulator.Application.Users;

/// <summary>Stores only a one-way hash of refresh-token bearer credentials.</summary>
public static class RefreshTokenHasher
{
    public static string Hash(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
    }
}
