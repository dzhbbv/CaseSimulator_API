using CaseSimulator.Domain.Common;

namespace CaseSimulator.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public Guid UserId { get; protected set; }
    public string Token { get; protected set; }
    public DateTime ExpiresAt { get; protected set; }
    public bool IsRevoked { get; protected set; } = false;
    public User User { get; protected set; }

    private RefreshToken() { }

    public RefreshToken(Guid userId, string token, DateTime expiresAt)
    {
        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
    }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => !IsRevoked && !IsExpired;

    public void Revoke() => IsRevoked = true;
}