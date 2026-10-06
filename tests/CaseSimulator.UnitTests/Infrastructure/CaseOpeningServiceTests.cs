using CaseSimulator.Domain.Entities;
using CaseSimulator.Domain.ValueObjects;
using CaseSimulator.Infrastructure.Services;
using Xunit;
using FluentAssertions;

namespace CaseSimulator.UnitTests.Infrastructure;

public class CaseOpeningServiceTests
{
    private static User CreateUser() => new User(
        "testuser", "hash", "test@mail.com",
        "clientseed", "serverseed123", "serverhash");

    private static Case CreateConfiguredCase()
    {
        var caseEntity = new Case("testcase", "url", new Money(10));
        caseEntity.AddItem(new CaseItem("Common Item", "url", Rarity.Common, new Money(1)), 0.7m);
        caseEntity.AddItem(new CaseItem("Rare Item", "url", Rarity.Rare, new Money(50)), 0.2m);
        caseEntity.AddItem(new CaseItem("Celestial Item", "url", Rarity.Celestial, new Money(500)), 0.1m);
        return caseEntity;
    }

    [Fact]
    public void OpenCase_ShouldReturnItem()
    {
        var service = new CaseOpeningService();
        var user = CreateUser();
        var caseEntity = CreateConfiguredCase();

        var result = service.OpenCase(caseEntity, user);

        result.Should().NotBeNull();
        result.CaseItem.Should().NotBeNull();
        result.Round.Should().NotBeNull();
    }

    [Fact]
    public void OpenCase_ReturnedItem_ShouldBelongToCase()
    {
        var service = new CaseOpeningService();
        var user = CreateUser();
        var caseEntity = CreateConfiguredCase();
        var itemIds = caseEntity.CaseContent.Select(c => c.CaseItemId).ToList();

        var result = service.OpenCase(caseEntity, user);

        itemIds.Should().Contain(result.CaseItem.Id);
    }

    [Fact]
    public void OpenCase_ShouldIncrementNonce()
    {
        var service = new CaseOpeningService();
        var user = CreateUser();
        var caseEntity = CreateConfiguredCase();
        var nonceBefore = user.CurrentNonce;

        service.OpenCase(caseEntity, user);

        user.CurrentNonce.Should().Be(nonceBefore + 1);
    }

    [Fact]
    public void OpenCase_StatisticalDistribution_ShouldMatchProbabilities()
    {
        var service = new CaseOpeningService();
        var caseEntity = CreateConfiguredCase();
        var items = caseEntity.CaseContent
            .Select(x => x.CaseItem)
            .ToList();

        var counts = items.ToDictionary(item => item.Id, _ => 0);
    
        const int iterations = 10000;
        for (int i = 0; i < iterations; i++)
        {
            var user = new User(
                "user" + i,
                "hash",
                $"user{i}@mail.com",
                "client" + i,
                "server" + i,
                "hash" + i);

            var result = service.OpenCase(caseEntity, user);

            counts[result.CaseItem.Id]++;
        }
        
        var commonRate = (double)counts[items[0].Id] / iterations;
        var rareRate = (double)counts[items[1].Id] / iterations;
        var celestialRate = (double)counts[items[2].Id] / iterations;

        commonRate.Should().BeApproximately(0.7, 0.05);
        rareRate.Should().BeApproximately(0.2, 0.05);
        celestialRate.Should().BeApproximately(0.1, 0.05);
    }
}