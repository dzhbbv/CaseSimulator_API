using CaseSimulator.Domain.Entities;
using CaseSimulator.Domain.Exception;
using CaseSimulator.Domain.ValueObjects;
using Xunit;
using FluentAssertions;

namespace CaseSimulator.UnitTests;

public class UserTests
{
    private static User CreateUser() => new User(
        "testuser",
        "hashedpassword",
        "test@example.com",
        "clientseed123",
        "serverseed123",
        "serverhash123");

    [Fact]
    public void User_WhenCreated_BalanceShouldBeZero()
    {
        var user = CreateUser();
        user.Balance.Should().Be(Money.Zero);
    }

    [Fact]
    public void User_Deposit_ShouldIncreaseBalance()
    {
        var user = CreateUser();

        user.Deposit(new Money(100));

        user.Balance.Amount.Should().Be(100);
    }

    [Fact]
    public void User_MultipleDeposits_ShouldAccumulate()
    {
        var user = CreateUser();

        user.Deposit(new Money(100));
        user.Deposit(new Money(50));

        user.Balance.Amount.Should().Be(150);
    }

    [Fact]
    public void User_Withdraw_ShouldDecreaseBalance()
    {
        var user = CreateUser();
        user.Deposit(new Money(100));

        user.Withdraw(new Money(30));

        user.Balance.Amount.Should().Be(70);
    }

    [Fact]
    public void User_Withdraw_WhenInsufficientBalance_ShouldThrow()
    {
        var user = CreateUser();
        user.Deposit(new Money(50));

        var act = () => user.Withdraw(new Money(100));

        act.Should().Throw<InsufficientBalanceException>();
    }

    [Fact]
    public void User_SpendOnCase_WhenInsufficientBalance_ShouldThrow()
    {
        var user = CreateUser();

        var act = () => user.SpendOnCase(new Money(100));

        act.Should().Throw<InsufficientBalanceException>();
    }

    [Fact]
    public void User_SpendOnCase_ShouldDecreaseBalance()
    {
        var user = CreateUser();
        user.Deposit(new Money(100));

        user.SpendOnCase(new Money(25));

        user.Balance.Amount.Should().Be(75);
    }

    [Fact]
    public void User_Deposit_ShouldCreateTransaction()
    {
        var user = CreateUser();

        user.Deposit(new Money(100));

        user.Transactions.Should().HaveCount(1);
    }
}