using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SwiftScale.BuildingBlocks;
using SwiftScale.BuildingBlocks.Messaging;
using SwiftScale.Modules.Ordering.Application.Interfaces;
using SwiftScale.Modules.Ordering.Domain;
using SwiftScale.Modules.Ordering.Domain.Outbox;

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

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("ordering");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderingDbContext).Assembly);

        modelBuilder.Entity<OutboxMessage>(builder =>
        {
            builder.ToTable("OutboxMessages", "ordering");
            builder.HasKey(x => x.Id);
        });

        modelBuilder.Entity<InboxMessage>(builder =>
        {
            builder.ToTable("InboxMessages", "ordering"); // Keep it in the ordering schema
            builder.HasKey(x => x.Id);
        });

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

        var outboxMessages = domainEvents.Select(domainEvent => new OutboxMessage
        {
            Id = Guid.NewGuid(),
            OccurredOnUtc = DateTime.UtcNow,
            Type = domainEvent.GetType().Name,
            Content = JsonConvert.SerializeObject(domainEvent, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All // Important for deserialization later!
            })
        }).ToList();

        OutboxMessages.AddRange(outboxMessages);

        return await base.SaveChangesAsync(ct);


        //// 2. Save the changes to the DB first
        //var result = await base.SaveChangesAsync(ct);

        //// 3. Dispatch the events through MediatR
        //foreach (var domainEvent in domainEvents)
        //{
        //    await _publisher.Publish(domainEvent, ct);
        //}

        //return result;
    }
}