using MediatR;
using Microsoft.Extensions.Logging;
using SwiftScale.Modules.Identity.Application.RegisterUser;

namespace SwiftScale.Modules.Ordering.Application.Users
{
    internal sealed class UserRegisteredIntegrationEventHandler(ILogger<UserRegisteredIntegrationEventHandler> logger)
    : INotificationHandler<UserRegisteredIntegrationEvent>
    {
        public Task Handle(UserRegisteredIntegrationEvent notification, CancellationToken cancellationToken)
        {
            // Architect's Note: This runs independently of the Catalog handler
            logger.LogInformation("Ordering Module: Preparing welcome discount for new user {Email}", notification.Email);

            // Future Logic: _discountService.ApplyNewUserCoupon(notification.UserId);

            return Task.CompletedTask;
        }
    }
}
