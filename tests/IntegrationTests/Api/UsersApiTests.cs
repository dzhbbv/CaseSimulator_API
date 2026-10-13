using System.Net;
using CaseSimulator.IntegrationTests.Infrastructure;
using FluentAssertions;
using Xunit;

namespace CaseSimulator.IntegrationTests.Api;

public sealed class UsersApiTests(IntegrationTestFactory factory)
    : IClassFixture<IntegrationTestFactory>
{
    [Theory]
    [InlineData("/api/users/balance")]
    [InlineData("/api/users/inventory")]
    [InlineData("/api/users/transactions")]
    [InlineData("/api/users/sales")]
    public async Task ProtectedEndpoints_WithoutToken_ReturnUnauthorized(string endpoint)
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync(endpoint);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
