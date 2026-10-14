using CaseSimulator.Application.Cases.Queries.GetCaseById;
using CaseSimulator.Application.Interfaces;
using CaseSimulator.Domain.Entities;
using CaseSimulator.Domain.Exception;
using CaseSimulator.Domain.ValueObjects;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace CaseSimulator.UnitTests.Application.Cases;

public class GetCaseByIdQueryHandlerTests
{
    [Fact]
    public async Task WhenCaseExists_ReturnCaseAndAllItems()
    {
        var firstItem = new CaseItem("First item", "first.png", Rarity.Rare, new Money(10m));
        var secondItem = new CaseItem("Second item", "second.png", Rarity.Epic, new Money(20m));
        var caseEntity = new Case("Detail case", "case.png", new Money(75m));
        caseEntity.AddItem(firstItem, 0.4m);
        caseEntity.AddItem(secondItem, 0.6m);
        var mockDb = new Mock<IApplicationDbContext>();
        mockDb.Setup(db => db.Cases)
            .Returns(new List<Case> { caseEntity }.BuildMockDbSet().Object);

        var handler = new GetCaseByIdQueryHandler(mockDb.Object);
        var result = await handler.Handle(new GetCaseByIdQuery(caseEntity.Id), CancellationToken.None);

        result.Id.Should().Be(caseEntity.Id);
        result.Name.Should().Be("detail case");
        result.Price.Should().Be(75m);
        result.ImageUrl.Should().Be("case.png");
        result.Items.Should().HaveCount(2);
        result.Items.Should().ContainSingle(x => x.CaseItemId == firstItem.Id && x.Name == "first item" && x.Price == 10m && x.Rarity == "Rare");
        result.Items.Should().ContainSingle(x => x.CaseItemId == secondItem.Id && x.Name == "second item" && x.Price == 20m && x.Rarity == "Epic");
    }

    [Fact]
    public async Task WhenCaseDoesNotExist_ThrowNotFoundException()
    {
        var mockDb = new Mock<IApplicationDbContext>();
        mockDb.Setup(db => db.Cases)
            .Returns(new List<Case>().BuildMockDbSet().Object);
        var handler = new GetCaseByIdQueryHandler(mockDb.Object);

        var act = () => handler.Handle(new GetCaseByIdQuery(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
