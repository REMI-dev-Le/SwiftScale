using Microsoft.EntityFrameworkCore;
using SwiftScale.BuildingBlocks.Messaging;
using SwiftScale.Modules.Payment.Application.Interfaces;
using SwiftScale.Modules.Payment.Domain;

namespace SwiftScale.Modules.Payment.Infrastructure;

// src/Modules/Payment/Infrastructure/PaymentDbContext.cs
public class PaymentDbContext : DbContext, IPaymentDbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options)
        : base(options)
    {
    }

    public DbSet<Domain.Payment> Payments
    {
        get => Set<Domain.Payment>();
    }

    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 1. Core Payment Entity
        modelBuilder.Entity<Domain.Payment>(builder =>
        {
            builder.ToTable("Payments", "payment"); // Explicit schema
            builder.HasKey(p => p.Id);
        });

        // 2. Reliability Tables
        modelBuilder.Entity<InboxMessage>(builder =>
        {
            builder.ToTable("InboxMessages", "payment");
            builder.HasKey(m => m.Id);
        });

        modelBuilder.Entity<OutboxMessage>(builder =>
        {
            builder.ToTable("OutboxMessages", "payment");
            builder.HasKey(m => m.Id);
        });
        modelBuilder.HasDefaultSchema("payment");
    }
}