using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SwiftScale.Modules.Ordering.Application.Interfaces;

namespace SwiftScale.Modules.Ordering.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddOrderingInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<OrderingDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("Database"),
                    npgsqlOptions => npgsqlOptions.MigrationsAssembly(typeof(OrderingDbContext).Assembly.FullName)));

            services.AddScoped<IOrderingDbContext>(sp => sp.GetRequiredService<OrderingDbContext>());

            services.AddMediatR(config =>
                config.RegisterServicesFromAssembly(typeof(IOrderingDbContext).Assembly));

            return services;
        }
    }
}
