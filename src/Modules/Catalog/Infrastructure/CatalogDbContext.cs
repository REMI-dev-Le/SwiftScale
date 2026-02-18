using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftScale.BuildingBlocks;
using SwiftScale.Modules.Catalog.Application.Interfaces;
using SwiftScale.Modules.Catalog.Domain;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace SwiftScale.Modules.Catalog.Infrastructure;

// src/Modules/Catalog/Infrastructure/CatalogDbContext.cs
public class CatalogDbContext : DbContext, ICatalogDbContext
{
    private readonly IPublisher _publisher;
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options, IPublisher publisher)
        : base(options)
    {
        _publisher = publisher;
    }

    public DbSet<Event> Transactions
    {
        get => Set<Event>();
    }

    public DbSet<Product> Products
    {
        get => Set<Product>();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // 1. Extract events from entities before saving to DB
        var domainEntities = ChangeTracker.Entries<Entity>()
            .Where(x => x.Entity.GetDomainEvents().Any())
            .Select(x => x.Entity)
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.GetDomainEvents())
            .ToList();

        // 2. Clear events from the entity to prevent duplicate firing
        domainEntities.ForEach(x => x.ClearDomainEvents());

        // 3. Commit the data to PostgreSQL
        var result = await base.SaveChangesAsync(cancellationToken);

        // 4. Dispatch events to local handlers
        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent, cancellationToken);
        }

        return result;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("catalog");
        // This line automatically finds the ProductConfiguration file
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}