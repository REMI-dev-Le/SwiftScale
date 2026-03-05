using MediatR;


namespace SwiftScale.Modules.Payment.Contracts.IntegrationEvents
{
    public record PaymentFailedIntegrationEvent(Guid Id,
                                                Guid OrderId,
                                                string Reason,
                                                DateTime OccurredOnUtc) : INotification;
}
