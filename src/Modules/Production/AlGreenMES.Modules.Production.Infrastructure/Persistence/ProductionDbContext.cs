using AlGreenMES.Modules.Production.Application.Interfaces;
using AlGreenMES.Modules.Production.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AlGreenMES.Modules.Production.Infrastructure.Persistence;

public class ProductionDbContext : DbContext, IProductionUnitOfWork
{
    public DbSet<Process> Processes => Set<Process>();
    public DbSet<SubProcess> SubProcesses => Set<SubProcess>();
    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
    public DbSet<ProductCategoryProcess> ProductCategoryProcesses => Set<ProductCategoryProcess>();
    public DbSet<ProductCategoryDependency> ProductCategoryDependencies => Set<ProductCategoryDependency>();
    public DbSet<SpecialRequestType> SpecialRequestTypes => Set<SpecialRequestType>();

    public ProductionDbContext(DbContextOptions<ProductionDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("production");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductionDbContext).Assembly);
    }
}
