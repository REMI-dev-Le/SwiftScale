using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SwiftScale.Modules.Payment.Application.Interfaces;
using SwiftScale.Modules.Payment.Application.Orders.HandleOrderPlaced;

namespace SwiftScale.Modules.Payment.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPaymentInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PaymentDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("Database"),
                    npgsqlOptions => npgsqlOptions.MigrationsAssembly(typeof(PaymentDbContext).Assembly.FullName)));

            services.AddScoped<IPaymentDbContext>(sp => sp.GetRequiredService<PaymentDbContext>());

            services.AddMediatR(config =>
                config.RegisterServicesFromAssembly(typeof(IPaymentDbContext).Assembly));

            services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(typeof(OrderPlacedIntegrationEventHandler).Assembly);
            });

            return services;
        }
    }
}
