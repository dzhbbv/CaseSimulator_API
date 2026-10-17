using CaseSimulator.Application.Interfaces;
using CaseSimulator.Application.Users.Commands;
using CaseSimulator.Domain.Entities;
using CaseSimulator.Domain.Exception;
using CaseSimulator.Domain.ValueObjects;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace CaseSimulator.UnitTests.Application.Users;

public class SaleItemCommandHandlerTests
{
    private static User CreateUserWithItem(out InventoryItem inventoryItem)
    {
        var user = new User("testuser", "hash", "test@mail.com", "client", "server", "server-hash");
        var caseItem = new CaseItem("test item", "item.png", Rarity.Rare, new Money(75m));
        inventoryItem = new InventoryItem(user.Id, caseItem.Id, caseItem);
        user.AddInventoryItem(inventoryItem);
        user.Deposit(new Money(100m));
        return user;
    }

    private static Mock<IApplicationDbContext> CreateDb(List<User> users)
    {
        var mockDb = new Mock<IApplicationDbContext>();
        mockDb.Setup(db => db.Users).Returns(users.BuildMockDbSet().Object);
        mockDb.Setup(db => db.Transactions).Returns(new List<Transaction>().BuildMockDbSet().Object);
        mockDb.Setup(db => db.SaleItems).Returns(new List<SaleItem>().BuildMockDbSet().Object);
        mockDb.Setup(db => db.InventoryItems).Returns(new List<InventoryItem>().BuildMockDbSet().Object);
        mockDb.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        return mockDb;
    }

    [Fact]
    public async Task WhenItemExists_SellItemIncreaseBalanceAndSaveHistory()
    {
        var user = CreateUserWithItem(out var inventoryItem);
        var mockDb = CreateDb([user]);
        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(x => x.UserId).Returns(user.Id);
        var handler = new SaleItemCommandHandler(mockDb.Object, currentUser.Object);

        var result = await handler.Handle(new SaleItemCommand(inventoryItem.Id), CancellationToken.None);

        result.Should().NotBeNull();
        result.Type.Should().Be(CaseSimulator.Domain.Enums.TransactionType.Sale);
        user.Balance.Amount.Should().Be(175m);
        user.InventoryItems.Should().BeEmpty();
        user.SaleHistory.Should().ContainSingle(x => x.CaseItemId == inventoryItem.CaseItemId && x.Price.Amount == 75m);
        mockDb.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WhenUserDoesNotExist_ThrowNotFoundException()
    {
        var mockDb = CreateDb([]);
        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(x => x.UserId).Returns(Guid.NewGuid());
        var handler = new SaleItemCommandHandler(mockDb.Object, currentUser.Object);

        var act = () => handler.Handle(new SaleItemCommand(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task WhenItemIsNotInInventory_ThrowNotFoundException()
    {
        var user = CreateUserWithItem(out _);
        var mockDb = CreateDb([user]);
        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(x => x.UserId).Returns(user.Id);
        var handler = new SaleItemCommandHandler(mockDb.Object, currentUser.Object);

        var act = () => handler.Handle(new SaleItemCommand(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
