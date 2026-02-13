using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace SwiftScale.Modules.Ordering.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddOrderingInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<OrderingDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("Database"),
                    npgsqlOptions => npgsqlOptions.MigrationsAssembly(typeof(OrderingDbContext).Assembly.FullName)));

            return services;
        }
    }
}
