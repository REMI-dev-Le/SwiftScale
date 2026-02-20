using SwiftScale.BuildingBlocks;

namespace SwiftScale.Modules.Ordering.Domain;

public class Order : Entity
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public OrderStatus Status { get; private set; }

    private readonly List<OrderItem> _items = new();
    public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();

    private Order() { }

    public static Order Create(Guid userId)
    {
        return new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            Status = OrderStatus.Pending
        };
    }

    public void AddItem(Guid productId, decimal price, int quantity)
    {
        _items.Add(new OrderItem(Id, productId, price, quantity));
    }
}
