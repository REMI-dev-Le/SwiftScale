using SwiftScale.BuildingBlocks;

namespace SwiftScale.Modules.Payment.Application.ProcessPayment
{
    public record PaymentCompletedIntegrationEvent(Guid OrderId,Guid PaymentId) : IIntegrationEvent
    {
        public Guid Id => Guid.NewGuid();
        public DateTime OccurredOnUtc => DateTime.UtcNow;
    }
}
