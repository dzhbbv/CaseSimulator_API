using CaseSimulator.Application.Admin.Commands;
using CaseSimulator.Application.Interfaces;
using CaseSimulator.Domain.Entities;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace CaseSimulator.UnitTests.Application.Admin;

public class CreateCaseItemCommandHandlerTests
{
    [Fact]
    public async Task WhenIncorrectPrice_ShouldThrowArgumentException()
    {
        var mockDb = new Mock<IApplicationDbContext>();
        
        mockDb.Setup(db => db.CaseItems)
            .Returns(new List<CaseItem>().BuildMockDbSet().Object);
        
        var handler = new CreateCaseItemCommandHandler(mockDb.Object);
        var act = async () => await handler.Handle(
            new CreateCaseItemCommand("testname", "testurl", "Common", -14.88m),
            CancellationToken.None);
        
        await act.Should().ThrowAsync<ArgumentException>();
    }
    
    [Fact]
    public async Task WhenUnknownRarityName_ShouldThrowArgumentException()
    {
        var mockDb = new Mock<IApplicationDbContext>();
        
        mockDb.Setup(db => db.CaseItems)
            .Returns(new List<CaseItem>().BuildMockDbSet().Object);
        
        var handler = new CreateCaseItemCommandHandler(mockDb.Object);
        var act = async () => await handler.Handle(
            new CreateCaseItemCommand("testname", "testurl", "unknownrarity", 14.88m),
            CancellationToken.None);
        
        await act.Should().ThrowAsync<ArgumentException>();
    }
    
    [Fact]
    public async Task SuccessfulCreateCaseItem_ShouldReturnID()
    {
        var mockDb = new Mock<IApplicationDbContext>();
        
        mockDb.Setup(db => db.CaseItems)
            .Returns(new List<CaseItem>().BuildMockDbSet().Object);
        
        mockDb.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        
        var handler = new CreateCaseItemCommandHandler(mockDb.Object);
        var result = await handler.Handle(
            new CreateCaseItemCommand("testname", "testurl", "Common", 14.88m),
            CancellationToken.None);
        
        result.Should().NotBe(Guid.Empty);
        mockDb.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}