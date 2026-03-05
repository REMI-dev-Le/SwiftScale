using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SwiftScale.BuildingBlocks.Messaging;
using SwiftScale.Modules.Ordering.Application.Interfaces;
using SwiftScale.Modules.Payment.Contracts.IntegrationEvents;

namespace SwiftScale.Modules.Ordering.Application.Payments.HandlePaymentFailed
{
    internal sealed class PaymentFailedIntegrationEventHandler(IOrderingDbContext context, ILogger<PaymentFailedIntegrationEventHandler> logger) : INotificationHandler<PaymentFailedIntegrationEvent>
    {
        public async Task Handle(PaymentFailedIntegrationEvent notification, CancellationToken ct)
        {
            // 1. Idempotency Check via Inbox
            if (await context.InboxMessages.AnyAsync(m => m.Id == notification.Id, ct))
            {
                return;
            }

            // 2. Fetch the Order
            var order = await context.Orders.FirstOrDefaultAsync(o => o.Id == notification.OrderId, ct);

            if (order is null)
            {
                logger.LogError("Order {OrderId} not found for failed payment {EventId}",
                    notification.OrderId, notification.Id);
                return;
            }

            // 3. Execute Domain Logic: Cancel the Order
            try
            {
                order.Cancel(notification.Reason);
                logger.LogWarning("Order {OrderId} cancelled: {Reason}", order.Id, notification.Reason);
            }
            catch (InvalidOperationException ex)
            {
                logger.LogError(ex, "Failed to cancel order {OrderId}", order.Id);
                // Even if cancellation fails, we mark the message as processed to avoid loops
            }

            // 4. Mark Message as Processed in Inbox
            context.InboxMessages.Add(new InboxMessage
            {
                Id = notification.Id,
                ProcessedOnUtc = DateTime.UtcNow,
                Type = nameof(PaymentFailedIntegrationEvent)
            });

            await context.SaveChangesAsync(ct);
        }
    }
}
