using AlGreenMES.Modules.Orders.Application.Interfaces;
using AlGreenMES.Modules.Orders.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AlGreenMES.Modules.Orders.Infrastructure.Persistence;

public class OrdersDbContext : DbContext, IOrdersUnitOfWork
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<OrderItemProcess> OrderItemProcesses => Set<OrderItemProcess>();
    public DbSet<OrderItemSubProcess> OrderItemSubProcesses => Set<OrderItemSubProcess>();
    public DbSet<OrderItemSpecialRequest> OrderItemSpecialRequests => Set<OrderItemSpecialRequest>();
    public DbSet<OrderItemSubProcessLog> OrderItemSubProcessLogs => Set<OrderItemSubProcessLog>();
    public DbSet<WorkSession> WorkSessions => Set<WorkSession>();
    public DbSet<ChangeRequest> ChangeRequests => Set<ChangeRequest>();
    public DbSet<BlockRequest> BlockRequests => Set<BlockRequest>();
    public DbSet<Notification> Notifications => Set<Notification>();

    public OrdersDbContext(DbContextOptions<OrdersDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("orders");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrdersDbContext).Assembly);
    }
}
