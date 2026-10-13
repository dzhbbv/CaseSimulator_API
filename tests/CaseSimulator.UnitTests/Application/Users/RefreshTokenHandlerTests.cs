using System.Reflection;
using CaseSimulator.Application.Interfaces;
using CaseSimulator.Application.Users.Commands;
using CaseSimulator.Domain.Entities;
using CaseSimulator.Domain.Exception;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace CaseSimulator.UnitTests.Application.Users;

public class RefreshTokenHandlerTests
{
    private static User CreateUser() => new("testuser", "hash", "test@mail.com", "client", "server", "server-hash");

    private static RefreshToken CreateToken(User user, string token, DateTime? expiresAt = null)
    {
        var refreshToken = new RefreshToken(user.Id, token, expiresAt ?? DateTime.UtcNow.AddDays(1));
        typeof(RefreshToken).GetProperty(nameof(RefreshToken.User), BindingFlags.Instance | BindingFlags.Public)!
            .SetValue(refreshToken, user);
        return refreshToken;
    }

    [Fact]
    public async Task WhenTokenIsActive_ReturnNewAuthResultRevokeOldAndSaveNewToken()
    {
        var user = CreateUser();
        var existingToken = CreateToken(user, "old-refresh-token");
        var mockDb = new Mock<IApplicationDbContext>();
        var mockJwt = new Mock<IJwtTokenService>();
        mockDb.Setup(db => db.RefreshTokens).Returns(new List<RefreshToken> { existingToken }.BuildMockDbSet().Object);
        mockDb.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        mockJwt.Setup(x => x.GenerateToken(user)).Returns("new-access-token");
        mockJwt.Setup(x => x.GenerateRefreshToken()).Returns("new-refresh-token");

        var handler = new RefreshTokenHandler(mockDb.Object, mockJwt.Object);
        var result = await handler.Handle(new RefreshTokenCommand("old-refresh-token"), CancellationToken.None);

        result.AccessToken.Should().Be("new-access-token");
        result.RefreshToken.Should().Be("new-refresh-token");
        existingToken.IsRevoked.Should().BeTrue();
        mockDb.Verify(db => db.RefreshTokens.AddAsync(It.Is<RefreshToken>(x => x.Token == "new-refresh-token" && x.UserId == user.Id), It.IsAny<CancellationToken>()), Times.Once);
        mockDb.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WhenTokenDoesNotExist_ThrowInvalidCredentialsException()
    {
        var mockDb = new Mock<IApplicationDbContext>();
        mockDb.Setup(db => db.RefreshTokens).Returns(new List<RefreshToken>().BuildMockDbSet().Object);
        var handler = new RefreshTokenHandler(mockDb.Object, Mock.Of<IJwtTokenService>());

        var act = () => handler.Handle(new RefreshTokenCommand("missing-token"), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidCredentialsException>();
    }

    [Fact]
    public async Task WhenTokenIsExpired_ThrowInvalidCredentialsException()
    {
        var user = CreateUser();
        var token = CreateToken(user, "expired-token", DateTime.UtcNow.AddMinutes(-1));
        var mockDb = new Mock<IApplicationDbContext>();
        mockDb.Setup(db => db.RefreshTokens).Returns(new List<RefreshToken> { token }.BuildMockDbSet().Object);
        var handler = new RefreshTokenHandler(mockDb.Object, Mock.Of<IJwtTokenService>());

        var act = () => handler.Handle(new RefreshTokenCommand("expired-token"), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidCredentialsException>();
    }

    [Fact]
    public async Task WhenTokenIsRevoked_ThrowInvalidCredentialsException()
    {
        var user = CreateUser();
        var token = CreateToken(user, "revoked-token");
        token.Revoke();
        var mockDb = new Mock<IApplicationDbContext>();
        mockDb.Setup(db => db.RefreshTokens).Returns(new List<RefreshToken> { token }.BuildMockDbSet().Object);
        var handler = new RefreshTokenHandler(mockDb.Object, Mock.Of<IJwtTokenService>());

        var act = () => handler.Handle(new RefreshTokenCommand("revoked-token"), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidCredentialsException>();
    }
}
