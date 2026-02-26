
namespace SwiftScale.BuildingBlocks.Messaging
{
    public sealed class InboxMessage
    {
        public Guid Id { get; set; } // This matches the Integration Event ID
        public string Type { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime OccurredOnUtc { get; set; }
        public DateTime? ProcessedOnUtc { get; set; }
        public string? Error { get; set; }
    }
}
