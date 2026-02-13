using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace SwiftScale.Modules.Payment.Infrastructure
{
    public class PaymentDbContextFactory : IDesignTimeDbContextFactory<PaymentDbContext>
    {
        public PaymentDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PaymentDbContext>();

            // Use a dummy connection string for design-time (migration generation)
            optionsBuilder.UseNpgsql("Host=localhost;Database=SwiftScaleDb;Username=app;Password=apppass");

            return new PaymentDbContext(optionsBuilder.Options);
        }
    }
}
