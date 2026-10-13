using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CaseSimulator.IntegrationTests.Infrastructure;
using FluentAssertions;
using Xunit;

namespace CaseSimulator.IntegrationTests.Api;

public sealed class CasesApiTests(IntegrationTestFactory factory)
    : IClassFixture<IntegrationTestFactory>
{
    [Fact]
    public async Task GetCases_ReturnsOkAndJsonArray()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/cases");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(json);
        document.RootElement.ValueKind.Should().Be(JsonValueKind.Array);
    }

    [Fact]
    public async Task GetCaseById_WhenMissing_ReturnsBadRequestFromDomainMiddleware()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync($"/api/cases/{Guid.NewGuid()}");

        // The current ExceptionHandlingMiddleware maps DomainException (including NotFoundException) to 400.
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task OpenCase_WithoutToken_ReturnsUnauthorized()
    {
        using var client = factory.CreateClient();

        var response = await client.PostAsync($"/api/cases/{Guid.NewGuid()}/open", content: null);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCaseById_WhenIdIsEmpty_ReturnsBadRequest()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync($"/api/cases/{Guid.Empty}");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
