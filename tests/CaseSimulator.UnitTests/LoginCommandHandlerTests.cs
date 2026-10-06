using CaseSimulator.Application.Interfaces;
using CaseSimulator.Application.Users.Commands;
using CaseSimulator.Domain.Entities;
using CaseSimulator.Domain.Exception;
using FluentAssertions;
using MockQueryable.Moq;
using Xunit;
using Moq;

namespace CaseSimulator.UnitTests;

public class LoginCommandHandlerTests
{
    private static User CreateUser() => new User(
        "testuser", "hashedpassword", "test@mail.com",
        "clientseed", "serverseed", "serverhash");

    [Fact]
    public async Task Login_WhenValidCredentials_ShouldReturnAuthResult()
    {
        var user = CreateUser();
        
        var mockDb = new Mock<IApplicationDbContext>();
        var mockPassword = new Mock<IPasswordService>();
        var mockJwt = new Mock<IJwtTokenService>();
        
        mockDb.Setup(db => db.Users)
            .Returns(new List<User> { user }.BuildMockDbSet().Object);
        
        mockDb.Setup(db => db.RefreshTokens)
            .Returns(new List<RefreshToken>().BuildMockDbSet().Object);
        
        mockPassword
            .Setup(p => p.Verify("correctpassword", "hashedpassword"))
            .Returns(true);
        
        mockJwt
            .Setup(j => j.GenerateToken(It.IsAny<User>()))
            .Returns("access-token");
        mockJwt
            .Setup(j => j.GenerateRefreshToken())
            .Returns("refresh-token");

        var handler = new LoginCommandHandler(mockDb.Object, mockPassword.Object, mockJwt.Object);
        
        var result = await handler.Handle(
            new LoginCommand("test@mail.com", "correctpassword"),
            CancellationToken.None);
        
        result.AccessToken.Should().Be("access-token");
        result.RefreshToken.Should().Be("refresh-token");
    }

    [Fact]
    public async Task Login_WhenWrongPassword_ShouldThrow()
    {
        var user = CreateUser();

        var mockDb = new Mock<IApplicationDbContext>();
        var mockPassword = new Mock<IPasswordService>();
        var mockJwt = new Mock<IJwtTokenService>();

        mockDb.Setup(db => db.Users)
            .Returns(new List<User> { user }.BuildMockDbSet().Object);
        
        mockPassword
            .Setup(p => p.Verify(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(false);

        var handler = new LoginCommandHandler(mockDb.Object, mockPassword.Object, mockJwt.Object);

        var act = async () => await handler.Handle(
            new LoginCommand("test@mail.com", "wrongpassword"),
            CancellationToken.None);

        await act.Should().ThrowAsync<InvalidCredentialsException>();
    }
}