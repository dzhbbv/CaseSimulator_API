using CaseSimulator.Domain.ValueObjects;
using Xunit;
using FluentAssertions;

namespace CaseSimulator.UnitTests;

public class MoneyTests
{
    [Fact]
    public void Money_WhenCreatedWithPositiveAmount_ShouldSucceed()
    {
        var money = new Money(100);
        
        money.Amount.Should().Be(100);
    }

    [Fact]
    public void Money_WhenCreatedWithZero_ShouldSucceed()
    {
        var money = new Money(0);
        money.Amount.Should().Be(0);
    }

    [Fact]
    public void Money_WhenCreatedWithNegativeAmount_ShouldThrow()
    {
        var act = () => new Money(-1);
        
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Money_Addition_ShouldReturnCorrectSum()
    {
        var a = new Money(100);
        var b = new Money(50);

        var result = a + b;

        result.Amount.Should().Be(150);
    }

    [Fact]
    public void Money_Subtraction_ShouldReturnCorrectDifference()
    {
        var a = new Money(100);
        var b = new Money(30);

        var result = a - b;

        result.Amount.Should().Be(70);
    }

    [Fact]
    public void Money_Zero_ShouldBeZero()
    {
        Money.Zero.Amount.Should().Be(0);
    }

    [Theory]
    [InlineData(100, 50, true)]
    [InlineData(50, 100, false)]
    [InlineData(100, 100, false)]
    public void Money_GreaterThan_ShouldWorkCorrectly(decimal a, decimal b, bool expected)
    {
        var moneyA = new Money(a);
        var moneyB = new Money(b);

        var result = moneyA > moneyB;

        result.Should().Be(expected);
    }
}