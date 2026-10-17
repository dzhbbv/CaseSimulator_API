using CaseSimulator.Application.Cases.Commands.OpenCase;
using CaseSimulator.Application.Common.Models;
using CaseSimulator.Application.Interfaces;
using CaseSimulator.Domain.Entities;
using CaseSimulator.Domain.Exception;
using CaseSimulator.Domain.ValueObjects;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace CaseSimulator.UnitTests.Application.Cases;

public class OpenCaseCommandHandlerTests
{
    private static User CreateUser(decimal balance = 500m)
    {
        var user = new User("testuser", "password-hash", "test@mail.com", "client", "server", "server-hash");
        if (balance > 0) user.Deposit(new Money(balance));
        return user;
    }

    private static CaseItem CreateItem() => new("test item", "item.png", Rarity.Rare, new Money(25m));

    private static Case CreateConfiguredCase(CaseItem item, decimal price = 100m, decimal chance = 1m)
    {
        var caseEntity = new Case("test case", "case.png", new Money(price));
        caseEntity.AddItem(item, chance);
        return caseEntity;
    }

    private static Mock<IApplicationDbContext> CreateDb(List<User> users, List<Case> cases)
    {
        var mockDb = new Mock<IApplicationDbContext>();
        mockDb.Setup(db => db.Users).Returns(users.BuildMockDbSet().Object);
        mockDb.Setup(db => db.Cases).Returns(cases.BuildMockDbSet().Object);
        mockDb.Setup(db => db.InventoryItems).Returns(new List<InventoryItem>().BuildMockDbSet().Object);
        mockDb.Setup(db => db.Transactions).Returns(new List<Transaction>().BuildMockDbSet().Object);
        mockDb.Setup(db => db.ProvablyFairRounds).Returns(new List<ProvablyFairRound>().BuildMockDbSet().Object);
        mockDb.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        return mockDb;
    }

    [Fact]
    public async Task WhenValid_ReturnWonItemAndPersistRoundAndInventory()
    {
        var user = CreateUser();
        var item = CreateItem();
        var caseEntity = CreateConfiguredCase(item);
        var round = new ProvablyFairRound(user.Id, caseEntity.Id, item.Id, "server", "server-hash", "client", 0);
        var mockDb = CreateDb([user], [caseEntity]);
        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(x => x.UserId).Returns(user.Id);
        var opening = new Mock<ICaseOpeningService>();
        opening.Setup(x => x.OpenCase(caseEntity, user)).Returns(new OpenCaseResult(item, round));

        var handler = new OpenCaseCommandHandler(mockDb.Object, currentUser.Object, opening.Object);
        var result = await handler.Handle(new OpenCaseCommand(caseEntity.Id), CancellationToken.None);

        result.Should().Be(item.Id);
        user.InventoryItems.Should().ContainSingle(x => x.CaseItemId == item.Id);
        mockDb.Verify(x => x.ProvablyFairRounds.AddAsync(round, It.IsAny<CancellationToken>()), Times.Once);
        mockDb.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WhenUserDoesNotExist_ThrowNotFoundException()
    {
        var mockDb = CreateDb([], []);
        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(x => x.UserId).Returns(Guid.NewGuid());
        var handler = new OpenCaseCommandHandler(mockDb.Object, currentUser.Object, Mock.Of<ICaseOpeningService>());

        var act = () => handler.Handle(new OpenCaseCommand(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task WhenCaseDoesNotExist_ThrowNotFoundException()
    {
        var user = CreateUser();
        var mockDb = CreateDb([user], []);
        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(x => x.UserId).Returns(user.Id);
        var handler = new OpenCaseCommandHandler(mockDb.Object, currentUser.Object, Mock.Of<ICaseOpeningService>());

        var act = () => handler.Handle(new OpenCaseCommand(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task WhenBalanceIsInsufficient_ThrowInsufficientBalanceException()
    {
        var user = CreateUser(0m);
        var caseEntity = CreateConfiguredCase(CreateItem(), 100m);
        var mockDb = CreateDb([user], [caseEntity]);
        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(x => x.UserId).Returns(user.Id);
        var handler = new OpenCaseCommandHandler(mockDb.Object, currentUser.Object, Mock.Of<ICaseOpeningService>());

        var act = () => handler.Handle(new OpenCaseCommand(caseEntity.Id), CancellationToken.None);

        await act.Should().ThrowAsync<InsufficientBalanceException>();
    }

    [Fact]
    public async Task WhenCaseIsNotConfigured_ThrowInvalidCaseConfigurationException()
    {
        var user = CreateUser();
        var caseEntity = CreateConfiguredCase(CreateItem(), chance: 0.5m);
        var mockDb = CreateDb([user], [caseEntity]);
        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(x => x.UserId).Returns(user.Id);
        var handler = new OpenCaseCommandHandler(mockDb.Object, currentUser.Object, Mock.Of<ICaseOpeningService>());

        var act = () => handler.Handle(new OpenCaseCommand(caseEntity.Id), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidCaseConfigurationException>();
    }
}
