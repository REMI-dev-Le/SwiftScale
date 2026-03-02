using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SwiftScale.BuildingBlocks.Messaging;
using SwiftScale.Modules.Ordering.Application.Interfaces;
using SwiftScale.Modules.Payment.Application.ProcessPayment;

namespace SwiftScale.Modules.Ordering.Application.Payments.HandlePaymentCompleted
{
    public sealed class PaymentCompletedInboxHandler(IOrderingDbContext context, ILogger<PaymentCompletedInboxHandler> logger) : INotificationHandler<PaymentCompletedIntegrationEvent>
    {

        public async Task Handle(PaymentCompletedIntegrationEvent notification, CancellationToken ct)
        {
            // 1. De-duplication Check
            if (await context.InboxMessages.AnyAsync(m => m.Id == notification.Id, ct))
            {
                logger.LogInformation("Payment event {EventId} already processed. Skipping.", notification.Id);
                return;
            }

            // 2. Business Logic: Update Order Status to 'Paid'
            var order = await context.Orders.FindAsync(notification.OrderId);
            if (order != null)
            {
                order.MarkAsPaid(); // Imagine a method that updates Status = Paid
            }
            else
            {
                    logger.LogError("Order {OrderId} not found for payment {EventId}", notification.OrderId, notification.Id);
                    return;
            }

            // 3. Persist the "Work Done" flag
            context.InboxMessages.Add(new InboxMessage
            {
                Id = notification.Id,
                OccurredOnUtc = DateTime.UtcNow,
                ProcessedOnUtc = DateTime.UtcNow,
                Type = nameof(PaymentCompletedIntegrationEvent),
                Content = JsonConvert.SerializeObject(notification)
            });

            await context.SaveChangesAsync(ct);

            logger.LogInformation("Order {OrderId} successfully marked as Paid via Inbox.", order.Id);
        }
    }
}
