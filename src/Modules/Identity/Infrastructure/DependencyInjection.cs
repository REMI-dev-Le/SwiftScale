// File: src/Modules/Identity/Infrastructure/DependencyInjection.cs
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SwiftScale.BuildingBlocks.Behaviors;
using SwiftScale.Modules.Identity.Application.Interfaces;
using SwiftScale.Modules.Identity.Application.RegisterUser;
using SwiftScale.Modules.Identity.Infrastructure;
using SwiftScale.Modules.Identity.Infrastructure.Security;
using static System.Net.Mime.MediaTypeNames;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");
        services.AddDbContext<IdentityDbContext>(options =>options.UseNpgsql(connectionString));
        services.AddScoped<IIdentityDbContext>(sp => sp.GetRequiredService<IdentityDbContext>());
        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();

        // 1. Register all validators found in the Application assembly
        services.AddValidatorsFromAssembly(typeof(RegisterUserCommand).Assembly);

        // 2. Add the Validation Behavior to the MediatR pipeline
        services.AddMediatR(config => {
            config.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly);
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        return services;
    }
}