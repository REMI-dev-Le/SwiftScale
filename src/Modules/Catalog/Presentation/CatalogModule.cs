using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SwiftScale.BuildingBlocks;
using SwiftScale.Modules.Catalog.Application.Products.CreateProduct;
using SwiftScale.Modules.Catalog.Application.Products.GetProduct;
using SwiftScale.Modules.Catalog.Application.UploadImage;
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

        group.MapPost("/products", async (CreateProductCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);

            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        });

        group.MapGet("/products/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetProductQuery(id));
            return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(result.Error);
        });

        group.MapPost("/{id:guid}/image", async (Guid id, IFormFile file, ISender sender) =>
        {
            using var stream = file.OpenReadStream();
            var command = new UploadProductImageCommand(id, stream, file.FileName);

            var result = await sender.Send(command);

            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        }).DisableAntiforgery(); // Required for file uploads in some Minimal API configs
        return endpoints;
    }
}