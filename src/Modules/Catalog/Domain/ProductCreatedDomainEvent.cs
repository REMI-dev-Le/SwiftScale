using SwiftScale.BuildingBlocks;
namespace SwiftScale.Modules.Catalog.Domain
{
    public record ProductCreatedDomainEvent(Guid ProductId) : IDomainEvent
    {
        public Guid Id => Guid.NewGuid();
        public DateTime OccurredOnUtc => DateTime.UtcNow;
    }
}
