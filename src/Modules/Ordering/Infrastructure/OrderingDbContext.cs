using Microsoft.EntityFrameworkCore;
using SwiftScale.Modules.Ordering.Application.Interfaces;
using SwiftScale.Modules.Ordering.Domain;

namespace SwiftScale.Modules.Ordering.Infrastructure;

// src/Modules/Ordering/Infrastructure/OrderingDbContext.cs
public class OrderingDbContext : DbContext, IOrderingDbContext
{
    public OrderingDbContext(DbContextOptions<OrderingDbContext> options)
        : base(options)
    {
    }

    public DbSet<Order> Orders
    {
        get => Set<Order>();
    }

    public DbSet<OrderItem> OrderItems
    {
        get => Set<OrderItem>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("ordering");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderingDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}