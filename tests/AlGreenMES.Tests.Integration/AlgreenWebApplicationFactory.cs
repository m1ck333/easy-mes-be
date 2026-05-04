using AlGreenMES.Modules.Identity.Infrastructure.Persistence;
using AlGreenMES.Modules.Orders.Infrastructure.Persistence;
using AlGreenMES.Modules.Production.Infrastructure.Persistence;
using AlGreenMES.Modules.Tenancy.Infrastructure.Persistence;
using AlgreenMES.API;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Testcontainers.PostgreSql;
using Xunit;

namespace AlGreenMES.Tests.Integration;

public class AlgreenWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("algreen_test")
        .WithUsername("test")
        .WithPassword("test")
        .Build();

    public string ConnectionString => _postgres.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _postgres.StopAsync();
        await base.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = ConnectionString,
                ["Cors:AllowedOrigins:0"] = "http://localhost:5173",
                ["JwtSettings:Secret"] = "TEST_SECRET_KEY_AT_LEAST_32_CHARACTERS_LONG_FOR_HS256",
                ["JwtSettings:Issuer"] = "AlGreenMES",
                ["JwtSettings:Audience"] = "AlGreenMES",
                ["JwtSettings:ExpirationMinutes"] = "60"
            });
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        using var scope = host.Services.CreateScope();
        var sp = scope.ServiceProvider;
        sp.GetRequiredService<TenancyDbContext>().Database.Migrate();
        sp.GetRequiredService<IdentityDbContext>().Database.Migrate();
        sp.GetRequiredService<ProductionDbContext>().Database.Migrate();
        sp.GetRequiredService<OrdersDbContext>().Database.Migrate();

        return host;
    }
}
