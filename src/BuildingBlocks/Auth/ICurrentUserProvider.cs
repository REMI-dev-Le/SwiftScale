namespace SwiftScale.BuildingBlocks.Auth
{
    public interface ICurrentUserProvider
    {
        Guid UserId { get; }
        string Email { get; }
        bool IsAuthenticated { get; }
    }
}
