// File: src/Modules/Identity/Infrastructure/DependencyInjection.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SwiftScale.Modules.Identity.Application.Interfaces;
using SwiftScale.Modules.Identity.Infrastructure;
using SwiftScale.Modules.Identity.Infrastructure.Security;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");
        services.AddDbContext<IdentityDbContext>(options =>options.UseNpgsql(connectionString));
        services.AddScoped<IIdentityDbContext>(sp => sp.GetRequiredService<IdentityDbContext>());
        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        return services;
    }
}