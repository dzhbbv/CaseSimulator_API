using CaseSimulator.Application.Interfaces;
using CaseSimulator.Application.Users.Queries;
using CaseSimulator.Domain.Entities;
using CaseSimulator.Domain.ValueObjects;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace CaseSimulator.UnitTests.Application.Users;

public class GetSaleHistoryQueryHandlerTests
{
    [Fact]
    public async Task WhenSalesExist_ReturnMappedSaleHistory()
    {
        var userId = Guid.NewGuid();
        var caseItem = new CaseItem("sold item", "sold.png", Rarity.Rare, new Money(80m));
        var sale = new SaleItem(userId, caseItem.Id, caseItem.Price);
        var mockDb = new Mock<IApplicationDbContext>();
        var currentUser = new Mock<ICurrentUserService>();
        mockDb.Setup(db => db.SaleItems).Returns(new List<SaleItem> { sale }.BuildMockDbSet().Object);
        currentUser.Setup(x => x.UserId).Returns(userId);
        var handler = new GetSaleHistoryQueryHandler(mockDb.Object, currentUser.Object);

        var result = await handler.Handle(new GetSaleHistoryQuery(), CancellationToken.None);

        result.Should().ContainSingle();
        result.Single().Id.Should().Be(sale.Id);
        result.Single().CaseItemId.Should().Be(caseItem.Id);
        result.Single().Price.Should().Be(80m);
        result.Single().SoldDate.Should().Be(sale.SoldDate);
    }

    [Fact]
    public async Task WhenNoSalesExist_ReturnEmptyCollection()
    {
        var mockDb = new Mock<IApplicationDbContext>();
        var currentUser = new Mock<ICurrentUserService>();
        mockDb.Setup(db => db.SaleItems).Returns(new List<SaleItem>().BuildMockDbSet().Object);
        currentUser.Setup(x => x.UserId).Returns(Guid.NewGuid());
        var handler = new GetSaleHistoryQueryHandler(mockDb.Object, currentUser.Object);

        var result = await handler.Handle(new GetSaleHistoryQuery(), CancellationToken.None);

        result.Should().BeEmpty();
    }
}
