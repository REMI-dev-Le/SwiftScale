using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SwiftScale.BuildingBlocks;
using SwiftScale.Modules.Identity.Application.RegisterUser;
using SwiftScale.Modules.Identity.Infrastructure;


namespace SwiftScale.Modules.Identity.Presentation;

public class UserEndpoints : IModule
{ 

    public IServiceCollection RegisterModule(IServiceCollection services, IConfiguration configuration)
    {
        // Calls the Infrastructure registration we wrote on Day 2
        services.AddIdentityInfrastructure(configuration);
        return services;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("users"); // Optional: groups all identity routes under /users

        group.MapPost("/", async (RegisterUserCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        });
        return app;
    }
}