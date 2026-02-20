using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SwiftScale.BuildingBlocks;
using SwiftScale.Modules.Payment.Application.ProcessPayment;
using SwiftScale.Modules.Payment.Infrastructure;

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
        var group = endpoints.MapGroup("payments");

        group.MapPost("/", async (ProcessPaymentCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        });

        return endpoints;
    }
}