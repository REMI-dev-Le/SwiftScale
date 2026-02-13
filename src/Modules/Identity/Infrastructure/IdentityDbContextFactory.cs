using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace SwiftScale.Modules.Identity.Infrastructure
{
    public class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
    {
        public IdentityDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();

            // Use a dummy connection string for design-time (migration generation)
            optionsBuilder.UseNpgsql("Host=localhost;Database=SwiftScaleDb;Username=app;Password=apppass");

            return new IdentityDbContext(optionsBuilder.Options);
        }
    }
}
