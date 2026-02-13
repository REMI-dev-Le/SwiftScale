using Microsoft.EntityFrameworkCore;
using SwiftScale.Modules.Payment.Domain;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace SwiftScale.Modules.Payment.Infrastructure;

// src/Modules/Payment/Infrastructure/PaymentDbContext.cs
public class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options)
        : base(options)
    {
    }

    public DbSet<Transaction> Transactions
    {
        get => Set<Transaction>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("payment");
    }
}