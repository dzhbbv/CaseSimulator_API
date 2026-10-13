using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CaseSimulator.IntegrationTests.Infrastructure;
using FluentAssertions;

namespace CaseSimulator.IntegrationTests.Api;

public sealed class AdminApiTests(IntegrationTestFactory factory)
    : IClassFixture<IntegrationTestFactory>
{
    [Fact]
    public async Task CreateCase_WithoutAdminKey_ReturnsForbidden()
    {
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/admin/cases", new
        {
            name = $"case-{Guid.NewGuid():N}",
            imageUrl = "https://example.test/case.png",
            price = 10m
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateCase_WithAdminKey_ReturnsId_AndCaseCanBeRead()
    {
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Admin-Key", IntegrationTestFactory.AdminKey);
        var name = $"case-{Guid.NewGuid():N}";

        var create = await client.PostAsJsonAsync("/api/admin/cases", new
        {
            name,
            imageUrl = "https://example.test/case.png",
            price = 10m
        });

        create.StatusCode.Should().Be(HttpStatusCode.OK);
        using var createJson = JsonDocument.Parse(await create.Content.ReadAsStringAsync());
        var id = createJson.RootElement.GetProperty("id").GetGuid();
        id.Should().NotBeEmpty();

        var read = await client.GetAsync($"/api/cases/{id}");

        read.StatusCode.Should().Be(HttpStatusCode.OK);
        using var readJson = JsonDocument.Parse(await read.Content.ReadAsStringAsync());
        readJson.RootElement.GetProperty("id").GetGuid().Should().Be(id);
        readJson.RootElement.GetProperty("name").GetString().Should().Be(name.ToLowerInvariant());
    }

    [Fact]
    public async Task DeleteCase_WithoutAdminKey_ReturnsForbidden()
    {
        using var client = factory.CreateClient();

        var response = await client.DeleteAsync($"/api/admin/cases/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
