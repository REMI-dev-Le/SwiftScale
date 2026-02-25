using MediatR;
using Microsoft.Extensions.Logging;
using SwiftScale.Modules.Ordering.Domain.Events;

namespace SwiftScale.Modules.Ordering.Application.Order.CreateOrder
{
    internal sealed class OrderCreatedDomainEventHandler(ILogger<OrderCreatedDomainEventHandler> logger)
    : INotificationHandler<OrderCreatedDomainEvent>
    {
        public Task Handle(OrderCreatedDomainEvent notification, CancellationToken ct)
        {
            // Example: Logic to prepare the shipment or log the audit
            logger.LogInformation("Domain Event: Order {OrderId} has been created and is ready for processing.", notification.OrderId);
            return Task.CompletedTask;
        }
    }
}
