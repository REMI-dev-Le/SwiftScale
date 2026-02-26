using MediatR;

namespace SwiftScale.Modules.Payment.Application.ProcessPayment
{
    public record PaymentCompletedIntegrationEvent(Guid Id,               // Unique ID for the Inbox to track
                                                   Guid OrderId,
                                                   decimal Amount,
                                                   DateTime OccurredOnUtc) : INotification;
}
