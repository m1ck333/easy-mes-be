using System.Net.Http.Headers;
using System.Net.Http.Json;
using AlGreenMES.Modules.Identity.Application.Services;
using AlGreenMES.Modules.Identity.Domain.Entities;
using AlGreenMES.Modules.Identity.Infrastructure.Persistence;
using AlGreenMES.Modules.Tenancy.Domain.Entities;
using AlGreenMES.Modules.Tenancy.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace AlGreenMES.Tests.Integration.Helpers;

public static class TestDataSeeder
{
    public const string DefaultPassword = "TestPass123!";

    public static async Task<SeededTenant> SeedTenantWithUserAsync(
        AlgreenWebApplicationFactory factory,
        UserRole role = UserRole.Admin,
        string? tenantCodeOverride = null,
        string? emailOverride = null)
    {
        using var scope = factory.Services.CreateScope();
        var sp = scope.ServiceProvider;
        var tenancyDb = sp.GetRequiredService<TenancyDbContext>();
        var identityDb = sp.GetRequiredService<IdentityDbContext>();
        var passwordHasher = sp.GetRequiredService<IPasswordHasher>();

        var code = tenantCodeOverride ?? $"T{Guid.NewGuid():N}".Substring(0, 8).ToUpperInvariant();
        var email = emailOverride ?? $"u-{Guid.NewGuid():N}".Substring(0, 10) + "@test.local";

        var tenant = Tenant.Create($"Tenant {code}", code);
        tenancyDb.Tenants.Add(tenant);
        await tenancyDb.SaveChangesAsync();

        var user = User.Create(tenant.Id, email, passwordHasher.HashPassword(DefaultPassword),
            "Test", "User", role);
        identityDb.Users.Add(user);
        await identityDb.SaveChangesAsync();

        return new SeededTenant(tenant.Id, tenant.Code, user.Id, email, DefaultPassword, role);
    }

    public static async Task<string> LoginAndGetTokenAsync(
        HttpClient client, string email, string password, string tenantCode)
    {
        var resp = await client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = email,
            Password = password,
            TenantCode = tenantCode
        });
        resp.EnsureSuccessStatusCode();
        var body = await resp.Content.ReadFromJsonAsync<LoginResponse>();
        if (body is null || string.IsNullOrEmpty(body.Token))
            throw new InvalidOperationException("Login returned no token");
        return body.Token;
    }

    public static async Task<HttpClient> AuthenticatedClientAsync(
        AlgreenWebApplicationFactory factory, SeededTenant t)
    {
        var client = factory.CreateClient();
        var token = await LoginAndGetTokenAsync(client, t.Email, t.Password, t.TenantCode);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    private sealed record LoginResponse(string Token, string RefreshToken);
}

public sealed record SeededTenant(
    Guid TenantId,
    string TenantCode,
    Guid UserId,
    string Email,
    string Password,
    UserRole Role);
