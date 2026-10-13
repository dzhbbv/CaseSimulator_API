using CaseSimulator.Application.Cases.Queries.GetAllCases;
using CaseSimulator.Application.Interfaces;
using CaseSimulator.Domain.Entities;
using CaseSimulator.Domain.ValueObjects;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace CaseSimulator.UnitTests.Application.Cases;

public class GetAllCasesQueryHandlerTests
{
    [Fact]
    public async Task WhenCasesExist_ReturnAllCasesWithMappedFields()
    {
        var first = new Case("Premium", "https://example.test/premium.png", new Money(150m));
        var second = new Case("Starter", "https://example.test/starter.png", new Money(0m));
        var mockDb = new Mock<IApplicationDbContext>();

        mockDb.Setup(db => db.Cases)
            .Returns(new List<Case> { first, second }.BuildMockDbSet().Object);

        var handler = new GetAllCasesQueryHandler(mockDb.Object);
        var result = await handler.Handle(new GetAllCasesQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
        result.Should().ContainSingle(x => x.Id == first.Id && x.Name == "premium" && x.Price == 150m && x.ImageUrl == "https://example.test/premium.png");
        result.Should().ContainSingle(x => x.Id == second.Id && x.Name == "starter" && x.Price == 0m && x.ImageUrl == "https://example.test/starter.png");
    }

    [Fact]
    public async Task WhenNoCasesExist_ReturnEmptyCollection()
    {
        var mockDb = new Mock<IApplicationDbContext>();
        mockDb.Setup(db => db.Cases)
            .Returns(new List<Case>().BuildMockDbSet().Object);

        var handler = new GetAllCasesQueryHandler(mockDb.Object);
        var result = await handler.Handle(new GetAllCasesQuery(), CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}
