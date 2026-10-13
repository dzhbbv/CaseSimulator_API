using CaseSimulator.Application.Interfaces;
using CaseSimulator.Application.Users.Queries;
using CaseSimulator.Domain.Entities;
using CaseSimulator.Domain.Exception;
using CaseSimulator.Domain.ValueObjects;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace CaseSimulator.UnitTests.Application.Users;

public class GetBalanceQueryHandlerTests
{
    [Fact]
    public async Task WhenUserExists_ReturnBalance()
    {
        var user = new User("testuser", "hash", "test@mail.com", "client", "server", "server-hash");
        user.Deposit(new Money(123.45m));
        var mockDb = new Mock<IApplicationDbContext>();
        var currentUser = new Mock<ICurrentUserService>();
        mockDb.Setup(db => db.Users).Returns(new List<User> { user }.BuildMockDbSet().Object);
        currentUser.Setup(x => x.UserId).Returns(user.Id);
        var handler = new GetBalanceQueryHandler(mockDb.Object, currentUser.Object);

        var result = await handler.Handle(new GetBalanceQuery(), CancellationToken.None);

        result.Should().Be(123.45m);
    }

    [Fact]
    public async Task WhenUserDoesNotExist_ThrowNotFoundException()
    {
        var mockDb = new Mock<IApplicationDbContext>();
        var currentUser = new Mock<ICurrentUserService>();
        mockDb.Setup(db => db.Users).Returns(new List<User>().BuildMockDbSet().Object);
        currentUser.Setup(x => x.UserId).Returns(Guid.NewGuid());
        var handler = new GetBalanceQueryHandler(mockDb.Object, currentUser.Object);

        var act = () => handler.Handle(new GetBalanceQuery(), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
