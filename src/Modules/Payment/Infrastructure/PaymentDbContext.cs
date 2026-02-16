using Microsoft.EntityFrameworkCore;
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

    public DbSet<Transaction> Transactions
    {
        get => Set<Transaction>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("payment");
    }
}