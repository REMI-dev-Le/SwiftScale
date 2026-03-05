using MediatR;

namespace SwiftScale.Modules.Payment.Contracts.IntegrationEvents
{
    public record PaymentCompletedIntegrationEvent(Guid Id,
                                                   Guid OrderId,
                                                   Guid PaymentId,
                                                   DateTime OccurredOnUtc) : INotification;
}
