using SwiftScale.BuildingBlocks;

namespace SwiftScale.Modules.Ordering.Domain.Events
{
    public record OrderCancelledDomainEvent(Guid OrderId,string Reason) : IDomainEvent
    {
        public Guid Id => OrderId;

        public DateTime OccurredOnUtc => new DateTime();
    }
}
