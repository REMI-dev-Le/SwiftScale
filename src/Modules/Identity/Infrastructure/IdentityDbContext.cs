// File: src/Modules/Identity/Infrastructure/Persistence/IdentityDbContext.cs
using Microsoft.EntityFrameworkCore;
using SwiftScale.Modules.Identity.Domain;

namespace SwiftScale.Modules.Identity.Infrastructure;

public class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Architect Tip: Strictly define the schema name
        modelBuilder.HasDefaultSchema("identity");

        // Define primary key and constraints
        modelBuilder.Entity<User>().HasKey(u => u.Id);
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

        base.OnModelCreating(modelBuilder);
    }
}