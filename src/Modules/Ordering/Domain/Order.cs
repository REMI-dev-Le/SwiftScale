namespace SwiftScale.Modules.Ordering.Domain;

public class Order
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid EventId { get; private set; }
    public DateTime OrderDate { get; private set; }
    public string Status { get; private set; } = "Pending";

    public static Order Create(Guid userId, Guid eventId)
    {
        return new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            EventId = eventId,
            OrderDate = DateTime.UtcNow,
        };
    }

}
