using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SwiftScale.BuildingBlocks;
using SwiftScale.Modules.Catalog.Infrastructure;


namespace SwiftScale.Modules.Catalog.Presentation;

public class CatalogModule : IModule
{
    public IServiceCollection RegisterModule(IServiceCollection services, IConfiguration configuration)
    {
        // Calls the Infrastructure registration we wrote on Day 2
        services.AddCatalogInfrastructure(configuration);
        return services;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("catalog");
        group.MapGet("/events", () => Results.Ok("Catalog Events List"));
        return endpoints;
    }
}