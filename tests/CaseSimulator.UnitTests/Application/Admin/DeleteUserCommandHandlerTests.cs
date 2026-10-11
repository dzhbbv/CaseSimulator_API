using CaseSimulator.Application.Admin.Commands;
using CaseSimulator.Application.Interfaces;
using CaseSimulator.Domain.Entities;
using CaseSimulator.Domain.Exception;
using CaseSimulator.Domain.ValueObjects;
using FluentAssertions;
using MockQueryable;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace CaseSimulator.UnitTests.Application.Admin;

public class DeleteUserCommandHandlerTests
{
    private User CreateUser() => new User("testname", "hash", "testemail", "clientseed", "serverseed", "hash");

    [Fact]
    public async Task WhenUserNotFound_ThrowsNotFoundException()
    {
        var user = CreateUser();
        
        var mockDb = new Mock<IApplicationDbContext>();
        
        mockDb.Setup(x => x.Users)
            .Returns(new List<User> { user }.BuildMockDbSet().Object);
        
        mockDb.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        
        var handler = new DeleteUserCommandHandler(mockDb.Object);
        
        var act = async () => await handler.Handle(new DeleteUserCommand(Guid.NewGuid()), CancellationToken.None);
        
        await act.Should().ThrowAsync<NotFoundException>();
    }
    
    [Fact]
    public async Task SuccessfulDeleteUser()
    {
        var user = CreateUser();
        
        var mockDb = new Mock<IApplicationDbContext>();
        
        mockDb.Setup(x => x.Users)
            .Returns(new List<User> { user }.BuildMockDbSet().Object);
        
        mockDb.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        
        var handler = new DeleteUserCommandHandler(mockDb.Object);
        
        await handler.Handle(new DeleteUserCommand(user.Id), CancellationToken.None);
        
        mockDb.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}