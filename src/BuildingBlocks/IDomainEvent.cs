using MediatR;

namespace SwiftScale.BuildingBlocks
{
    public interface IDomainEvent : INotification
    {
        Guid Id { get; }
        DateTime OccurredOnUtc { get; }
    }
}
