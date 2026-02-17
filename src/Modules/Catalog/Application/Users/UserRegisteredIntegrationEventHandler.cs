using MediatR;
using Microsoft.Extensions.Logging;
using SwiftScale.Modules.Identity.Application.RegisterUser;

namespace SwiftScale.Modules.Catalog.Application.Users
{
    internal sealed class UserRegisteredIntegrationEventHandler(ILogger<UserRegisteredIntegrationEventHandler> logger) 
        : INotificationHandler<UserRegisteredIntegrationEvent>
    {
        public Task Handle(UserRegisteredIntegrationEvent notification, CancellationToken cancellationToken)
        {
            // For Day 5, we will simply log this to verify it works
            logger.LogInformation("Catalog Module: Handling User Registered Event for {UserId}", notification.UserId);

            // In the future: context.Profiles.Add(new Profile { UserId = notification.UserId });

            return Task.CompletedTask;
        }
    }
}
