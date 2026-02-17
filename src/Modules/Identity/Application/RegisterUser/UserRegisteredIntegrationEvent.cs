using SwiftScale.BuildingBlocks;

namespace SwiftScale.Modules.Identity.Application.RegisterUser
{
    public record UserRegisteredIntegrationEvent(Guid UserId,string Email) : IIntegrationEvent
    {
        public Guid Id => Guid.NewGuid();
        public DateTime OccurredOnUtc => DateTime.UtcNow;
    }
}
