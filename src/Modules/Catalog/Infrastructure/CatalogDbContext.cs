using Microsoft.EntityFrameworkCore;
using SwiftScale.Modules.Catalog.Application.Interfaces;
using SwiftScale.Modules.Catalog.Domain;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace SwiftScale.Modules.Catalog.Infrastructure;

// src/Modules/Catalog/Infrastructure/CatalogDbContext.cs
public class CatalogDbContext : DbContext, ICatalogDbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options)
        : base(options)
    {
    }

    public DbSet<Event> Transactions
    {
        get => Set<Event>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("catalog");
        base.OnModelCreating(modelBuilder);
    }
}