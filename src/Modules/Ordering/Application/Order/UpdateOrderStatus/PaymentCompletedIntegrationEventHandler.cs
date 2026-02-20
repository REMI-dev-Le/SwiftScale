using MediatR;
using SwiftScale.Modules.Ordering.Application.Interfaces;
using SwiftScale.Modules.Payment.Application.ProcessPayment;

namespace SwiftScale.Modules.Ordering.Application.Order.UpdateOrderStatus
{
    internal sealed class PaymentCompletedIntegrationEventHandler(IOrderingDbContext context): INotificationHandler<PaymentCompletedIntegrationEvent>
    {
        public async Task Handle(PaymentCompletedIntegrationEvent notification, CancellationToken ct)
        {
            var order = await context.Orders.FindAsync([notification.OrderId], ct);

            if (order is not null)
            {
                // We'll add a method to the Order entity for this
                order.MarkAsPaid();
                await context.SaveChangesAsync(ct);
            }
        }
    }
}
