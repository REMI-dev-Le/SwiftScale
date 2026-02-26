using SwiftScale.BuildingBlocks;


namespace SwiftScale.Modules.Ordering.Domain.Events
{
    public record OrderPaidDomainEvent(Guid OrderId) : IDomainEvent
    {
        public Guid Id => OrderId;

        public DateTime OccurredOnUtc => new DateTime();
    }
}
