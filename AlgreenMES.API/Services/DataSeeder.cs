using AlGreenMES.Modules.Identity.Application.Services;
using AlGreenMES.Modules.Identity.Domain.Entities;
using AlGreenMES.Modules.Identity.Infrastructure.Persistence;
using AlGreenMES.Modules.Production.Domain.Entities;
using AlGreenMES.Modules.Production.Infrastructure.Persistence;
using AlGreenMES.Modules.Tenancy.Domain.Entities;
using AlGreenMES.Modules.Tenancy.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AlgreenMES.API.Services;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        var tenancyDb = services.GetRequiredService<TenancyDbContext>();
        var identityDb = services.GetRequiredService<IdentityDbContext>();
        var productionDb = services.GetRequiredService<ProductionDbContext>();
        var passwordHasher = services.GetRequiredService<IPasswordHasher>();

        // 1. Get or create Demo Tenant
        var tenant = await tenancyDb.Tenants.FirstOrDefaultAsync(t => t.Code == "DEMO");
        if (tenant == null)
        {
            tenant = Tenant.Create("Demo Company", "demo");
            tenancyDb.Tenants.Add(tenant);
            await tenancyDb.SaveChangesAsync();
        }

        var tenantId = tenant.Id;

        // 2. Get or create Admin User
        var adminUser = await identityDb.Users.FirstOrDefaultAsync(u => u.Email == "admin@demo.com" && u.TenantId == tenantId);
        if (adminUser == null)
        {
            var passwordHash = passwordHasher.HashPassword("Admin123!");
            adminUser = User.Create(tenantId, "admin@demo.com", passwordHash, "Admin", "User", UserRole.Admin);
            identityDb.Users.Add(adminUser);
            await identityDb.SaveChangesAsync();
        }
        else if (!passwordHasher.VerifyPassword("Admin123!", adminUser.PasswordHash))
        {
            // Reset password if hash is invalid
            adminUser.ChangePassword(passwordHasher.HashPassword("Admin123!"));
            await identityDb.SaveChangesAsync();
        }

        // 3. Get or create Processes A-K
        var existingProcesses = await productionDb.Processes
            .Where(p => p.TenantId == tenantId)
            .ToListAsync();

        var processDefs = new (string Code, string Name, int Order)[]
        {
            ("A", "Krojenje", 1),
            ("B", "Kantiranje", 2),
            ("C", "CNC", 3),
            ("D", "Bušenje", 4),
            ("E", "Montaža", 5),
            ("F", "Brušenje", 6),
            ("G", "Grundiranje", 7),
            ("H", "Farbanje", 8),
            ("I", "Lakiranje", 9),
            ("J", "Pakiranje", 10),
            ("K", "Kontrola kvaliteta", 11)
        };

        var processEntities = new List<Process>();
        var needsSave = false;
        foreach (var (code, name, order) in processDefs)
        {
            var existing = existingProcesses.FirstOrDefault(p => p.Code == code);
            if (existing != null)
            {
                processEntities.Add(existing);
            }
            else
            {
                var process = Process.Create(tenantId, code, name, order, adminUser.Id);
                processEntities.Add(process);
                productionDb.Processes.Add(process);
                needsSave = true;
            }
        }

        if (needsSave)
            await productionDb.SaveChangesAsync();

        // 4. Get or create Product Category "Vrata Pivot" with all processes
        var pivotDoors = await productionDb.ProductCategories
            .Include(c => c.Processes)
            .Include(c => c.Dependencies)
            .FirstOrDefaultAsync(c => c.Name == "Vrata Pivot" && c.TenantId == tenantId);

        if (pivotDoors == null)
        {
            pivotDoors = ProductCategory.Create(tenantId, "Vrata Pivot", "Pivot vrata - standardna kategorija", adminUser.Id);
            productionDb.ProductCategories.Add(pivotDoors);

            for (var i = 0; i < processEntities.Count; i++)
            {
                pivotDoors.AddProcess(processEntities[i].Id, i + 1);
            }

            // Add process dependencies:
            // Grundiranje (G) depends on Brušenje (F)
            // Farbanje (H) depends on Grundiranje (G)
            // Lakiranje (I) depends on Farbanje (H)
            var brusenje = processEntities.First(p => p.Code == "F");
            var grundiranje = processEntities.First(p => p.Code == "G");
            var farbanje = processEntities.First(p => p.Code == "H");
            var lakiranje = processEntities.First(p => p.Code == "I");

            pivotDoors.AddDependency(grundiranje.Id, brusenje.Id);
            pivotDoors.AddDependency(farbanje.Id, grundiranje.Id);
            pivotDoors.AddDependency(lakiranje.Id, farbanje.Id);

            await productionDb.SaveChangesAsync();
        }

        // 5. Get or create Special Request Types
        if (!await productionDb.SpecialRequestTypes.AnyAsync(s => s.Code == "PESK" && s.TenantId == tenantId))
        {
            var peskarenje = SpecialRequestType.Create(tenantId, "PESK", "Peskarenje", "Dodaje proces peskarenja prije farbanja");
            productionDb.SpecialRequestTypes.Add(peskarenje);

            var brusenje = processEntities.First(p => p.Code == "F");
            var grundiranje = processEntities.First(p => p.Code == "G");
            var farbanje = processEntities.First(p => p.Code == "H");
            var lakiranje = processEntities.First(p => p.Code == "I");

            var samoFarbanje = SpecialRequestType.Create(tenantId, "SF", "Samo farbanje", "Samo proces farbanja - preskače ostale procese");
            samoFarbanje.SetOnlyProcesses(
                brusenje.Id,
                grundiranje.Id,
                farbanje.Id,
                lakiranje.Id
            );
            productionDb.SpecialRequestTypes.Add(samoFarbanje);

            await productionDb.SaveChangesAsync();
        }
    }
}
