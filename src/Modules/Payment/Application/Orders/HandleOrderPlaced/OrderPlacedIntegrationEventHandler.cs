using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SwiftScale.BuildingBlocks.Messaging;
using SwiftScale.Modules.Ordering.Contracts.IntegrationEvents;
using SwiftScale.Modules.Payment.Application.Interfaces;
using SwiftScale.Modules.Payment.Domain;

namespace SwiftScale.Modules.Payment.Application.Orders.HandleOrderPlaced
{
    public sealed class OrderPlacedIntegrationEventHandler(IPaymentDbContext context, ILogger<OrderPlacedIntegrationEventHandler> logger) : INotificationHandler<OrderPlacedIntegrationEvent>
    {
        public async Task Handle(OrderPlacedIntegrationEvent notification, CancellationToken ct)
        {
            logger.LogInformation("Handling integration event: {EventId} for Order {OrderId}",
            notification.Id, notification.OrderId);

            // 1. Idempotency Check: Verify if this message was already processed
            if (await context.InboxMessages.AnyAsync(m => m.Id == notification.Id, ct))
            {
                logger.LogWarning("Message {MessageId} already processed. Skipping.", notification.Id);
                return;
            }

            // 2. Business Logic: Create the Pending Payment record
            // Note: Amount comes from the calculated TotalAmount in the event
            var payment = Domain.Payment.Create(
                notification.OrderId,
                notification.TotalAmount);

            context.Payments.Add(payment);

            // 3. Mark as Processed: Save the Message ID to the Inbox
            // This MUST happen in the same database transaction as the Payment creation
            var inboxMessage = new InboxMessage
            {
                Id = notification.Id,
                OccurredOnUtc = notification.OccurredOnUtc,
                ProcessedOnUtc = DateTime.UtcNow,
                Type = nameof(OrderPlacedIntegrationEvent)
            };

            context.InboxMessages.Add(inboxMessage);

            // 4. Atomic Commit
            await context.SaveChangesAsync(ct);

            logger.LogInformation("Successfully initiated pending payment for Order {OrderId}",
                notification.OrderId);
        }
    }
}
