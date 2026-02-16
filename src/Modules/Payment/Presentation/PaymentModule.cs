using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SwiftScale.BuildingBlocks;
using SwiftScale.Modules.Payment.Infrastructure;
using System.Reflection;

namespace SwiftScale.Modules.Payment.Presentation;

public class PaymentModule : IModule
{
    public IServiceCollection RegisterModule(IServiceCollection services, IConfiguration configuration)
    {
        services.AddPaymentInfrastructure(configuration);
        return services;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("payment");
        group.MapPost("/process", () => Results.Ok("Payment Processed"));
        return endpoints;
    }
}