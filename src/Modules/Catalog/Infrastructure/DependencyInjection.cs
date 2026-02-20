using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SwiftScale.BuildingBlocks.Storage;
using SwiftScale.Modules.Catalog.Application.Interfaces;
using SwiftScale.Modules.Catalog.Infrastructure.Storage;

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
            services.AddScoped<IFileStorageService, LocalFileStorageService>();
            services.AddScoped<IFileStorageService, LocalFileStorageService>();
            services.AddScoped<ICatalogApi, CatalogApi>();
            services.AddMediatR(config =>
                config.RegisterServicesFromAssembly(typeof(ICatalogDbContext).Assembly));
            return services;
        }
    }
}
