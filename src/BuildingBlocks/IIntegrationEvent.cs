using MediatR;
namespace SwiftScale.BuildingBlocks
{
    // We inherit from INotification because MediatR uses this for 1-to-many "shouting"
    public interface IIntegrationEvent : INotification
    {
        Guid Id { get; }
        DateTime OccurredOnUtc { get; }
    }
}
