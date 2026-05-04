using System.Net;
using System.Net.Http.Json;
using AlGreenMES.Tests.Integration.Helpers;
using FluentAssertions;
using Xunit;

namespace AlGreenMES.Tests.Integration;

public class AuthTests : IntegrationTestBase
{
    public AuthTests(AlgreenWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        var t = await TestDataSeeder.SeedTenantWithUserAsync(Factory);

        var resp = await Client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = t.Email,
            Password = t.Password,
            TenantCode = t.TenantCode
        });

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await resp.Content.ReadFromJsonAsync<LoginBody>();
        body!.Token.Should().NotBeNullOrEmpty();
        body.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_WithInvalidPassword_Returns400_InvalidCredentials()
    {
        var t = await TestDataSeeder.SeedTenantWithUserAsync(Factory);

        var resp = await Client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = t.Email,
            Password = "WrongPassword!",
            TenantCode = t.TenantCode
        });

        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var err = await resp.Content.ReadFromJsonAsync<ErrorBody>();
        err!.Error.Code.Should().Be("INVALID_CREDENTIALS");
    }

    [Fact]
    public async Task Login_WithNonexistentUser_Returns400_InvalidCredentials()
    {
        var t = await TestDataSeeder.SeedTenantWithUserAsync(Factory);

        var resp = await Client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = $"missing-{Guid.NewGuid():N}@test.local",
            Password = "AnyPassword!",
            TenantCode = t.TenantCode
        });

        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var err = await resp.Content.ReadFromJsonAsync<ErrorBody>();
        err!.Error.Code.Should().Be("INVALID_CREDENTIALS");
    }

    private sealed record LoginBody(string Token, string RefreshToken);
    private sealed record ErrorBody(ErrorPayload Error);
    private sealed record ErrorPayload(string Code, string Message);
}
