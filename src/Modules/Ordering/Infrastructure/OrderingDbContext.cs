using Microsoft.EntityFrameworkCore;
using SwiftScale.Modules.Ordering.Domain;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace SwiftScale.Modules.Ordering.Infrastructure;

// src/Modules/Ordering/Infrastructure/OrderingDbContext.cs
public class OrderingDbContext : DbContext
{
    public OrderingDbContext(DbContextOptions<OrderingDbContext> options)
        : base(options)
    {
    }

    public DbSet<Order> Transactions
    {
        get => Set<Order>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("ordering");
    }
}