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

public class CreateCaseCommandHandlerTests
{
    [Fact]
    public async Task WhenEmptyName_ThrowArgumentNullException()
    {
        var mockDb = new Mock<IApplicationDbContext>();
        
        mockDb.Setup(db => db.Cases)
            .Returns(new List<Case>() {}.BuildMockDbSet().Object);
        
        var handler = new CreateCaseCommandHandler(mockDb.Object);
        
        var act = async () => await handler.Handle(
            new CreateCaseCommand(null!,  "testutl", 0.5m),
            CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
    
    [Fact]
    public async Task WhenCaseAlreadyExists_ThrowAlreadyExistingException()
    {
        var existingCase = new Case("testname", "testurl", new Money(100));
    
        var mockDb = new Mock<IApplicationDbContext>();
        mockDb.Setup(db => db.Cases)
            .Returns(new List<Case> { existingCase }.BuildMockDbSet().Object);
    
        var handler = new CreateCaseCommandHandler(mockDb.Object);
    
        // не нужен первый вызов — кейс уже "есть" в моке
        var act = async () => await handler.Handle(
            new CreateCaseCommand("testname", "testurl", 100m),
            CancellationToken.None);

        await act.Should().ThrowAsync<AlreadyExistingException>();
    }
    
    [Fact]
    public async Task SuccessfulCreateCase_ShouldReturnID()
    {
        var mockDb = new Mock<IApplicationDbContext>();
        
        mockDb.Setup(db => db.Cases)
            .Returns(new List<Case>() {}.BuildMockDbSet().Object);
        
        mockDb.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        
        var handler = new CreateCaseCommandHandler(mockDb.Object);
        var result = await handler.Handle(
            new CreateCaseCommand("testname", "testurl", 0.5m),
            CancellationToken.None);
        
        result.Should().NotBe(Guid.Empty);
        mockDb.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}