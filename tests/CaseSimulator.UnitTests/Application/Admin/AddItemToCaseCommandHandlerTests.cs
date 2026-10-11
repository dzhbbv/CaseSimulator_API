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

public class AddItemToCaseCommandHandlerTests
{
    private static Case CreateCase() =>
        new("casename", "testurl", new Money(150));

    private static CaseItem CreateCaseItem() =>
        new("testitem", "testurl", Rarity.Rare, new Money(50));

    private static Mock<IApplicationDbContext> CreateMockDb(
        List<Case> cases,
        List<CaseItem> items,
        List<CaseContent> contents)
    {
        var mockDb = new Mock<IApplicationDbContext>();

        var casesDbSet = cases.BuildMockDbSet();
        var itemsDbSet = items.BuildMockDbSet();
        var contentsDbSet = contents.BuildMockDbSet();

        mockDb.Setup(db => db.Cases)
            .Returns(casesDbSet.Object);

        mockDb.Setup(db => db.CaseItems)
            .Returns(itemsDbSet.Object);

        mockDb.Setup(db => db.CaseContents)
            .Returns(contentsDbSet.Object);

        mockDb.Setup(db => db.CaseContents.AddAsync(
                It.IsAny<CaseContent>(),
                It.IsAny<CancellationToken>()))
            .Callback<CaseContent, CancellationToken>(
                (content, _) => contents.Add(content));
        
        mockDb.Setup(db => db.SaveChangesAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        return mockDb;
    }

    [Fact]
    public async Task Handle_WhenTotalDropChanceExceedsOne_ThrowsInvalidCaseConfigurationException()
    {
        var caseEntity = CreateCase();
        var caseItem1 = CreateCaseItem();
        var caseItem2 = CreateCaseItem();

        var contents = new List<CaseContent>();

        var mockDb = CreateMockDb(
            [caseEntity],
            [caseItem1, caseItem2],
            contents);

        var handler = new AddItemToCaseCommandHandler(mockDb.Object);

        await handler.Handle(
            new AddItemToCaseCommand(caseEntity.Id, caseItem1.Id, 0.5m),
            CancellationToken.None);

        var act = () => handler.Handle(
            new AddItemToCaseCommand(caseEntity.Id, caseItem2.Id, 0.6m),
            CancellationToken.None);

        await act.Should()
            .ThrowAsync<InvalidCaseConfigurationException>();
    }

    [Fact]
    public async Task Handle_WhenItemAlreadyExists_ThrowsAlreadyExistingException()
    {
        var caseEntity = CreateCase();
        var caseItem = CreateCaseItem();

        var contents = new List<CaseContent>();

        var mockDb = CreateMockDb(
            [caseEntity],
            [caseItem],
            contents);

        var handler = new AddItemToCaseCommandHandler(mockDb.Object);

        await handler.Handle(
            new AddItemToCaseCommand(caseEntity.Id, caseItem.Id, 0.5m),
            CancellationToken.None);

        var act = () => handler.Handle(
            new AddItemToCaseCommand(caseEntity.Id, caseItem.Id, 0.5m),
            CancellationToken.None);

        await act.Should()
            .ThrowAsync<AlreadyExistingException>();
    }

    [Fact]
    public async Task Handle_WhenItemDoesNotExist_ThrowsNotFoundException()
    {
        var caseEntity = CreateCase();
        var existingItem = CreateCaseItem();

        var mockDb = CreateMockDb(
            [caseEntity],
            [existingItem],
            []);

        var handler = new AddItemToCaseCommandHandler(mockDb.Object);

        var act = () => handler.Handle(
            new AddItemToCaseCommand(
                caseEntity.Id,
                Guid.NewGuid(),
                0.5m),
            CancellationToken.None);

        await act.Should()
            .ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WhenCaseDoesNotExist_ThrowsNotFoundException()
    {
        var caseItem = CreateCaseItem();

        var mockDb = CreateMockDb(
            [],
            [caseItem],
            []);

        var handler = new AddItemToCaseCommandHandler(mockDb.Object);

        var act = () => handler.Handle(
            new AddItemToCaseCommand(
                Guid.NewGuid(),
                caseItem.Id,
                0.5m),
            CancellationToken.None);

        await act.Should()
            .ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WhenInputIsValid_SavesChanges()
    {
        var caseEntity = CreateCase();
        var caseItem = CreateCaseItem();

        var mockDb = CreateMockDb(
            [caseEntity],
            [caseItem],
            []);

        var handler = new AddItemToCaseCommandHandler(mockDb.Object);

        await handler.Handle(
            new AddItemToCaseCommand(caseEntity.Id, caseItem.Id, 0.5m),
            CancellationToken.None);

        mockDb.Verify(
            db => db.CaseContents.AddAsync(
                It.IsAny<CaseContent>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        mockDb.Verify(
            db => db.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}