using CaseSimulator.Application.Interfaces;
using CaseSimulator.Application.Users.Queries;
using CaseSimulator.Domain.Entities;
using CaseSimulator.Domain.ValueObjects;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace CaseSimulator.UnitTests.Application.Users;

public class GetUserInventoryQueryHandlerTests
{
    [Fact]
    public async Task WhenInventoryHasItems_ReturnMappedItems()
    {
        var user = new User("testuser", "hash", "test@mail.com", "client", "server", "server-hash");
        var item = new CaseItem("Rare item", "rare.png", Rarity.Rare, new Money(42m));
        var inventoryItem = new InventoryItem(user.Id, item.Id, item);
        var mockDb = new Mock<IApplicationDbContext>();
        var currentUser = new Mock<ICurrentUserService>();
        mockDb.Setup(db => db.InventoryItems).Returns(new List<InventoryItem> { inventoryItem }.BuildMockDbSet().Object);
        currentUser.Setup(x => x.UserId).Returns(user.Id);
        var handler = new GetUserInventoryQueryHandler(mockDb.Object, currentUser.Object);

        var result = await handler.Handle(new GetUserInventoryQuery(), CancellationToken.None);

        result.Should().ContainSingle();
        result.Single().Id.Should().Be(inventoryItem.Id);
        result.Single().ItemName.Should().Be("rare item");
        result.Single().Price.Should().Be(42m);
        result.Single().ImageUrl.Should().Be("rare.png");
        result.Single().RarityName.Should().Be("Rare");
        result.Single().RarityColor.Should().Be("#4b69ff");
    }

    [Fact]
    public async Task WhenInventoryIsEmpty_ReturnEmptyCollection()
    {
        var mockDb = new Mock<IApplicationDbContext>();
        var currentUser = new Mock<ICurrentUserService>();
        mockDb.Setup(db => db.InventoryItems).Returns(new List<InventoryItem>().BuildMockDbSet().Object);
        currentUser.Setup(x => x.UserId).Returns(Guid.NewGuid());
        var handler = new GetUserInventoryQueryHandler(mockDb.Object, currentUser.Object);

        var result = await handler.Handle(new GetUserInventoryQuery(), CancellationToken.None);

        result.Should().BeEmpty();
    }
}
