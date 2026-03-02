using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using SwiftScale.Modules.Ordering.Infrastructure;
using Testcontainers.PostgreSql;

namespace IntegrationTests
{
    public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
                                        .WithImage("postgres:16-alpine") // Use a specific, lightweight version
                                        .WithDatabase("swiftscale_test")
                                        .WithUsername("postgres")
                                        .WithPassword("postgres")
                                        .Build();

        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();

            // Manually trigger migrations
            using var scope = Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<OrderingDbContext>();
            await dbContext.Database.MigrateAsync();
        }

        public new async Task DisposeAsync() => await _dbContainer.StopAsync();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<OrderingDbContext>));
                if (descriptor != null) services.Remove(descriptor);

                services.AddDbContext<OrderingDbContext>(options =>
                {
                    options.UseNpgsql(_dbContainer.GetConnectionString());

                    // Add this to suppress the migration check during tests
                    options.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
                });
            });
        }
    }
}
