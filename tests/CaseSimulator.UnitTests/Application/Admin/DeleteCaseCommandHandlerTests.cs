using CaseSimulator.Application.Admin.Commands;
using CaseSimulator.Application.Interfaces;
using CaseSimulator.Domain.Entities;
using CaseSimulator.Domain.Exception;
using CaseSimulator.Domain.ValueObjects;
using FluentAssertions;
using MockQueryable.Moq;
using Xunit;
using Moq;

namespace CaseSimulator.UnitTests.Application.Admin;

public class DeleteCaseCommandHandlerTests
{
    private Case CreateCase() => new Case("testname", "testurl", new Money(67));
    
    [Fact]
    public async Task WhenCaseNotFound_ThrowCaseNotFoundException()
    {
        var caseEntity = CreateCase();
        var mockDb = new Mock<IApplicationDbContext>();

        mockDb.Setup(db => db.Cases)
            .Returns(new List<Case>() { caseEntity }.BuildMockDbSet().Object);
        
        mockDb.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        
        var handler = new DeleteCaseCommandHandler(mockDb.Object);
        
        var act = async () => await handler.Handle(
            new DeleteCaseCommand(Guid.NewGuid()), CancellationToken.None);
        
        await act.Should().ThrowAsync<NotFoundException>();
    }
    
    [Fact]
    public async Task SuccessfulDeleteCase()
    {
        var caseEntity = CreateCase();
        var mockDb = new Mock<IApplicationDbContext>();

        mockDb.Setup(db => db.Cases)
            .Returns(new List<Case>() { caseEntity }.BuildMockDbSet().Object);
        
        mockDb.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        
        var handler = new DeleteCaseCommandHandler(mockDb.Object);
        
        await handler.Handle(
            new DeleteCaseCommand(caseEntity.Id), CancellationToken.None);
        
        mockDb.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}