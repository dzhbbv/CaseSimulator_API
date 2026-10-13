using CaseSimulator.Application.Interfaces;
using CaseSimulator.Application.Users.Queries;
using CaseSimulator.Domain.Entities;
using CaseSimulator.Domain.Enums;
using CaseSimulator.Domain.ValueObjects;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace CaseSimulator.UnitTests.Application.Users;

public class GetTransactionHistoryQueryHandlerTests
{
    [Fact]
    public async Task WhenTransactionsExist_ReturnMappedTransactionsNewestFirst()
    {
        var userId = Guid.NewGuid();
        var older = new Transaction(userId, new Money(0m), new Money(25m), TransactionType.Deposit);
        Thread.Sleep(2);
        var newer = new Transaction(userId, new Money(25m), new Money(10m), TransactionType.CaseOpen);
        var mockDb = new Mock<IApplicationDbContext>();
        var currentUser = new Mock<ICurrentUserService>();
        mockDb.Setup(db => db.Transactions).Returns(new List<Transaction> { older, newer }.BuildMockDbSet().Object);
        currentUser.Setup(x => x.UserId).Returns(userId);
        var handler = new GetTransactionHistoryQueryHandler(mockDb.Object, currentUser.Object);

        var result = await handler.Handle(new GetTransactionHistoryQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
        result.First().Id.Should().Be(newer.Id);
        result.First().Amount.Should().Be(10m);
        result.First().CurrentBalance.Should().Be(25m);
        result.First().BalanceAfter.Should().Be(15m);
        result.First().Type.Should().Be(TransactionType.CaseOpen);
        result.Last().Id.Should().Be(older.Id);
        result.Last().BalanceAfter.Should().Be(25m);
    }

    [Fact]
    public async Task WhenNoTransactionsExist_ReturnEmptyCollection()
    {
        var mockDb = new Mock<IApplicationDbContext>();
        var currentUser = new Mock<ICurrentUserService>();
        mockDb.Setup(db => db.Transactions).Returns(new List<Transaction>().BuildMockDbSet().Object);
        currentUser.Setup(x => x.UserId).Returns(Guid.NewGuid());
        var handler = new GetTransactionHistoryQueryHandler(mockDb.Object, currentUser.Object);

        var result = await handler.Handle(new GetTransactionHistoryQuery(), CancellationToken.None);

        result.Should().BeEmpty();
    }
}
