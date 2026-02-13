using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace SwiftScale.Modules.Ordering.Infrastructure
{
    
    public class OrderDbContextFactory: IDesignTimeDbContextFactory<OrderingDbContext>
    {
        public OrderingDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<OrderingDbContext>();

            // Use a dummy connection string for design-time (migration generation)
            optionsBuilder.UseNpgsql("Host=localhost;Database=SwiftScaleDb;Username=app;Password=apppass");

            return new OrderingDbContext(optionsBuilder.Options);
        }
    }
}
