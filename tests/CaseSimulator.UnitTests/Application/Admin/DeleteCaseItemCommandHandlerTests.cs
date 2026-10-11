using CaseSimulator.Application.Admin.Commands;
using CaseSimulator.Application.Interfaces;
using CaseSimulator.Domain.Entities;
using CaseSimulator.Domain.Exception;
using CaseSimulator.Domain.ValueObjects;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace CaseSimulator.UnitTests.Application.Admin;

public class DeleteCaseItemCommandHandlerTests
{
    private Case CreateCase() => new Case("testname", "testurl", new Money(150));
    private CaseItem CreateCaseItem() => new CaseItem("testname", "testurl", Rarity.Rare, new Money(67));

    [Fact]
    public async Task WhenCaseItemNotFound_ThrowNotFoundException()
    {
        var caseEntity = CreateCase();
        var caseItem = CreateCaseItem();
        var mockDb = new Mock<IApplicationDbContext>();
        
        mockDb.Setup(x => x.Cases)
            .Returns(new List<Case>() { caseEntity }.BuildMockDbSet().Object);
        mockDb.Setup(x => x.CaseItems)
            .Returns(new List<CaseItem>() { caseItem }.BuildMockDbSet().Object);
        mockDb.Setup(x => x.CaseContents)
            .Returns(new List<CaseContent>().BuildMockDbSet().Object);
        
        var handler = new DeleteCaseItemCommandHandler(mockDb.Object);
        
        var act = async () => await handler.Handle(new DeleteCaseItemCommand(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
    
    [Fact]
    public async Task SuccessfulDeleteCaseItem()
    {
        var caseItem = CreateCaseItem();
        var caseEntity = CreateCase();

        caseEntity.AddItem(caseItem, 0.5m);

        var caseContents = caseEntity.CaseContent.ToList();

        caseContents.Should().ContainSingle();

        var mockCaseItems = new List<CaseItem> { caseItem }.BuildMockDbSet();
        var mockCaseContents = caseContents.BuildMockDbSet();

        var mockDb = new Mock<IApplicationDbContext>();

        mockDb.Setup(x => x.CaseItems)
            .Returns(mockCaseItems.Object);

        mockDb.Setup(x => x.CaseContents)
            .Returns(mockCaseContents.Object);

        mockDb.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new DeleteCaseItemCommandHandler(mockDb.Object);
        
        await handler.Handle(new DeleteCaseItemCommand(caseItem.Id), CancellationToken.None);

        mockDb.Verify(x => x.CaseContents.RemoveRange(It.Is<IEnumerable<CaseContent>>(
                contents => contents.Contains(caseContents[0]))), Times.Once);

        mockDb.Verify(x => x.CaseItems.Remove(caseItem),
            Times.Once);

        mockDb.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}