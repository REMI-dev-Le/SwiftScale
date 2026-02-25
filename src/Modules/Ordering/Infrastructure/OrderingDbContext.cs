using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftScale.BuildingBlocks;
using SwiftScale.Modules.Ordering.Application.Interfaces;
using SwiftScale.Modules.Ordering.Domain;

namespace SwiftScale.Modules.Ordering.Infrastructure;

// src/Modules/Ordering/Infrastructure/OrderingDbContext.cs
public class OrderingDbContext : DbContext, IOrderingDbContext
{
    private readonly IPublisher _publisher;
    public OrderingDbContext(DbContextOptions<OrderingDbContext> options, IPublisher publisher)
        : base(options)
    {
        _publisher = publisher;
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

    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        // 1. Get all entities that have events
        var domainEvents = ChangeTracker.Entries<Entity>()
            .Select(e => e.Entity)
            .SelectMany(e =>
            {
                var events = e.DomainEvents.ToList();
                e.ClearDomainEvents();
                return events;
            }).ToList();

        // 2. Save the changes to the DB first
        var result = await base.SaveChangesAsync(ct);

        // 3. Dispatch the events through MediatR
        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent, ct);
        }

        return result;
    }
}