using CaseSimulator.Application.Interfaces;
using CaseSimulator.Application.Users.Commands;
using CaseSimulator.Domain.Entities;
using CaseSimulator.Domain.Exception;
using FluentAssertions;
using MockQueryable.Moq;
using Xunit;
using Moq;

namespace CaseSimulator.UnitTests.Application.Users;

public class RegisterUserCommandHandlerTests
{
    private static User CreateUser() => new User(
        "testuser", "hashedpassword", "test@mail.com",
        "clientseed", "serverseed", "serverhash");

    [Fact]
    public async Task RegisterWhenUserAlreadyRegistered_ShouldThrowAlreadyExistingException()
    {
        var user = CreateUser();
        var mockDb = new Mock<IApplicationDbContext>();
        var mockPassword = new Mock<IPasswordService>();
        
        mockDb.Setup(db => db.Users)
            .Returns(new List<User>() {user}.BuildMockDbSet().Object);
        
        mockPassword.Setup(p => p.Hash(It.IsAny<string>()))
            .Returns("hashedpassword");
        
        var handler = new RegisterUserCommandHandler(mockDb.Object, mockPassword.Object);
        
        var act = async () => await handler.Handle(
            new RegisterUserCommand(user.Username, user.Email, "somepassword", user.ClientSeed),
            CancellationToken.None);
        
        await act.Should().ThrowAsync<AlreadyExistingException>();
    }
    
    [Fact]
    public async Task RegisterWhenUserDoesNotExist_ShouldCreateUser()
    {
        var mockDb = new Mock<IApplicationDbContext>();
        var mockPassword = new Mock<IPasswordService>();

        mockPassword
            .Setup(p => p.Hash(It.IsAny<string>()))
            .Returns("hashedpassword");
        
        mockDb.Setup(db => db.Users)
            .Returns(new List<User>().BuildMockDbSet().Object );
        
        var handler = new RegisterUserCommandHandler(mockDb.Object, mockPassword.Object);

        await handler.Handle(
            new RegisterUserCommand("testuser", "test@mail.com", "somepassword", "clientseed"),
            CancellationToken.None);
        
        mockDb.Verify(db => db.Users.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}