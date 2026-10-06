using Xunit;
using FluentAssertions;
using CaseSimulator.Domain.Entities;
using CaseSimulator.Domain.Exception;
using CaseSimulator.Domain.ValueObjects;

namespace CaseSimulator.UnitTests.Domain;

public class CaseTests
{
    private static Case CreateCase() => new Case("testcase", "testurl", new Money(100));
    
    [Fact]
    public void Case_WhenAddedDuplicateItem_ShouldThrow()
    {
        var caseEntity = CreateCase();
        var caseItem = new CaseItem("testitem", "testurl", Rarity.Rare, new Money(50));

        caseEntity.AddItem(caseItem, 0.5m);
        var act = () => caseEntity.AddItem(caseItem, 0.5m);
        
        act.Should().Throw<AlreadyExistingException>();
    }
    
    [Fact]
    public void Case_WhenDropChanceExceedsOne_ShouldThrow()
    {
        var caseEntity = CreateCase();
        caseEntity.AddItem(
            new CaseItem("testitem1", "testurl1", Rarity.Rare, new Money(50)), 0.5m);
        caseEntity.AddItem(
            new CaseItem("testitem2", "testurl2", Rarity.Celestial, new Money(250)), 0.3m);
        var act = () => caseEntity.AddItem(
            new CaseItem("testitem3", "testurl3", Rarity.Epic, new Money(500)), 0.5m);
        
        act.Should().Throw<InvalidCaseConfigurationException>();
    }

    [Fact]
    public void Case_WhenSumOfChancesEqualsOne_ShouldBeConfiguredCorrectly()
    {
        var caseEntity = CreateCase();
        caseEntity.AddItem(
            new CaseItem("testitem1", "testurl1", Rarity.Rare, new Money(50)), 0.5m);
        caseEntity.AddItem(
            new CaseItem("testitem2", "testurl2", Rarity.Celestial, new Money(250)), 0.3m);
        caseEntity.AddItem(
            new CaseItem("testitem3", "testurl3", Rarity.Epic, new Money(500)), 0.2m);
        
        caseEntity.IsConfiguredCorrectly().Should().Be(true);
    }
    
    
    [Fact]
    public void Сase_WhereSumOfProbabilities_LowerThanOne()
    {
        var caseEntity = CreateCase();
        caseEntity.AddItem(
            new CaseItem("testitem1", "testurl1", Rarity.Rare, new Money(50)), 0.5m);
        caseEntity.AddItem(
            new CaseItem("testitem2", "testurl2", Rarity.Celestial, new Money(250)), 0.3m);
        caseEntity.AddItem(
            new CaseItem("testitem3", "testurl3", Rarity.Epic, new Money(500)), 0.1999m);
        
        caseEntity.IsConfiguredCorrectly().Should().Be(false);
    }
    
    [Fact]
    public void Case_WhenAddedItems_ShouldContainSomeItems()
    {
        var caseEntity = CreateCase();
        caseEntity.AddItem(
            new CaseItem("testitem1", "testurl1", Rarity.Rare, new Money(50)), 0.5m);
        caseEntity.AddItem(
            new CaseItem("testitem2", "testurl2", Rarity.Celestial, new Money(250)), 0.4m);
        caseEntity.AddItem(
            new CaseItem("testitem3", "testurl3", Rarity.Epic, new Money(500)), 0.1m);
        
        caseEntity.CaseContent.Count.Should().Be(3);
    }
    
    [Fact]
    public void Case_WhenCreated_ShouldBe_NotConfiguredCorrectly()
    {
        var caseEntity = CreateCase();
        caseEntity.IsConfiguredCorrectly().Should().BeFalse();
    }
}