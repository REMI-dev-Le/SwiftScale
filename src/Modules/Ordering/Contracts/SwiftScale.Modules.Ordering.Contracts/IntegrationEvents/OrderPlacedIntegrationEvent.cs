using MediatR;

namespace SwiftScale.Modules.Ordering.Contracts.IntegrationEvents
{
    public record OrderPlacedIntegrationEvent(Guid Id,
                                              Guid OrderId,
                                              Guid CustomerId,
                                              decimal TotalAmount,
                                              DateTime OccurredOnUtc) : INotification;
}
