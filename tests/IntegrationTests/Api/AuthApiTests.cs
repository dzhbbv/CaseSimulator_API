using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CaseSimulator.IntegrationTests.Infrastructure;
using FluentAssertions;
using Xunit;

namespace CaseSimulator.IntegrationTests.Api;

public sealed class AuthApiTests(IntegrationTestFactory factory)
    : IClassFixture<IntegrationTestFactory>
{
    [Fact]
    public async Task Register_ThenLogin_WithValidCredentials_ReturnsTokens()
    {
        using var client = factory.CreateClient();
        var suffix = Guid.NewGuid().ToString("N");
        var email = $"{suffix}@example.test";
        const string password = "Integration-Test-Password-123!";

        var register = await client.PostAsJsonAsync("/api/auth/register", new
        {
            username = $"user_{suffix}",
            email,
            password,
            clientSeed = "integration-test-seed"
        });

        register.StatusCode.Should().Be(HttpStatusCode.OK);
        using var registerJson = JsonDocument.Parse(await register.Content.ReadAsStringAsync());
        registerJson.RootElement.GetProperty("userId").GetGuid().Should().NotBeEmpty();

        var login = await client.PostAsJsonAsync("/api/auth/login", new { email, password });

        login.StatusCode.Should().Be(HttpStatusCode.OK);
        using var loginJson = JsonDocument.Parse(await login.Content.ReadAsStringAsync());
        loginJson.RootElement.GetProperty("accessToken").GetString().Should().NotBeNullOrWhiteSpace();
        loginJson.RootElement.GetProperty("refreshToken").GetString().Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Register_DuplicateEmail_ReturnsBadRequest()
    {
        using var client = factory.CreateClient();
        var suffix = Guid.NewGuid().ToString("N");
        var request = new
        {
            username = $"user_{suffix}",
            email = $"{suffix}@example.test",
            password = "Integration-Test-Password-123!",
            clientSeed = "integration-test-seed"
        };

        (await client.PostAsJsonAsync("/api/auth/register", request))
            .StatusCode.Should().Be(HttpStatusCode.OK);

        var duplicate = await client.PostAsJsonAsync("/api/auth/register", new
        {
            username = $"other_{suffix}",
            request.email,
            request.password,
            request.clientSeed
        });

        // Current middleware maps AlreadyExistingException to 400, not 409.
        duplicate.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithWrongPassword_ReturnsBadRequest()
    {
        using var client = factory.CreateClient();
        var suffix = Guid.NewGuid().ToString("N");
        var email = $"{suffix}@example.test";
        var password = "Integration-Test-Password-123!";

        var register = await client.PostAsJsonAsync("/api/auth/register", new
        {
            username = $"user_{suffix}",
            email,
            password,
            clientSeed = "integration-test-seed"
        });
        register.StatusCode.Should().Be(HttpStatusCode.OK);

        var login = await client.PostAsJsonAsync("/api/auth/login", new
        {
            email,
            password = "definitely-wrong-password"
        });

        // Current middleware maps InvalidCredentialsException to 400.
        login.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
