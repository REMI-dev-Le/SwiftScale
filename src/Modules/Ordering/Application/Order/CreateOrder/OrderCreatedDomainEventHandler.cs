using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SwiftScale.BuildingBlocks.Messaging;
using SwiftScale.Modules.Ordering.Application.Interfaces;
using SwiftScale.Modules.Ordering.Contracts.IntegrationEvents;
using SwiftScale.Modules.Ordering.Domain.Events;

namespace SwiftScale.Modules.Ordering.Application.Order.CreateOrder
{
    internal sealed class OrderCreatedDomainEventHandler(IOrderingDbContext context, ILogger<OrderCreatedDomainEventHandler> logger)
    : INotificationHandler<OrderCreatedDomainEvent>
    {
        public async Task Handle(OrderCreatedDomainEvent notification, CancellationToken ct)
        {
            // 1. Fetch the order details to get the TotalAmount and CustomerId
            var order = await context.Orders.FindAsync(notification.OrderId);

            if (order is null) return;

            // 2. Map to the NEW Integration Event from the Contracts project
            var integrationEvent = new OrderPlacedIntegrationEvent(
                Guid.NewGuid(),
                order.Id,
                order.UserId,
                order.TotalAmount,
                DateTime.UtcNow);

            // 3. Save to Outbox (This ensures reliability)
            // Your OutboxProcessor will eventually pick this up and publish it via MediatR/ServiceBus
            context.OutboxMessages.Add(new OutboxMessage
            {
                Id = integrationEvent.Id,
                Type = typeof(OrderPlacedIntegrationEvent).FullName,
                Content = JsonConvert.SerializeObject(integrationEvent),
                OccurredOnUtc = DateTime.UtcNow
            });

            await context.SaveChangesAsync(ct);
        }
    }
}
