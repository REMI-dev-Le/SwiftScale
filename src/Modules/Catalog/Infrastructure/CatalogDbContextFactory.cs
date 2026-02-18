using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace SwiftScale.Modules.Catalog.Infrastructure
{
    public class CatalogDbContextFactory: IDesignTimeDbContextFactory<CatalogDbContext>
    {
        public CatalogDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CatalogDbContext>();

            // Use a dummy connection string for design-time (migration generation)
            optionsBuilder.UseNpgsql("Host=localhost;Database=SwiftScaleDb;Username=app;Password=apppass");

            return new CatalogDbContext(optionsBuilder.Options,null!);
        }
    }
}
