using AlGreenMES.Tests.Integration.Helpers;
using FluentAssertions;
using Xunit;

namespace AlGreenMES.Tests.Integration.CrossTenant;

public class NotificationsCrossTenantTests : IntegrationTestBase
{
    public NotificationsCrossTenantTests(AlgreenWebApplicationFactory factory) : base(factory) { }

    // The audit flagged NotificationsController for taking userId from query string and
    // querying by userId only — no tenant scope. Tenant A user passes tenant B's userId
    // in the query and should NOT see tenant B's notifications. Sprint 2.4 (HasQueryFilter)
    // closes this implicitly at the DbContext layer.
    [Fact]
    public async Task GetNotifications_WithOtherTenantUserIdInQuery_DoesNotReturnOtherTenantNotifications()
    {
        var (tenantA, tenantB) = await TestDataSeeder.SeedTwoTenantsAsync(Factory);
        await TestDataSeeder.SeedNotificationAsync(Factory, tenantB.TenantId, tenantB.UserId, title: "B-NOTIF-MARKER");
        var clientA = await TestDataSeeder.AuthenticatedClientAsync(Factory, tenantA);

        var response = await clientA.GetAsync($"/api/notifications?userId={tenantB.UserId}");
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();
        body.Should().NotContain("B-NOTIF-MARKER",
            "tenant A must not see tenant B's notifications by passing tenant B's userId as a query param");
    }
}
