using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SwiftScale.Modules.Catalog.Application.Interfaces;

namespace SwiftScale.Modules.Catalog.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCatalogInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<CatalogDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("Database"),
                    npgsqlOptions => npgsqlOptions.MigrationsAssembly(typeof(CatalogDbContext).Assembly.FullName)));

            services.AddScoped<ICatalogDbContext>(sp => sp.GetRequiredService<CatalogDbContext>());

            services.AddMediatR(config =>
                config.RegisterServicesFromAssembly(typeof(ICatalogDbContext).Assembly));
            return services;
        }
    }
}
