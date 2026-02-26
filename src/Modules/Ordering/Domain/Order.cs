using SwiftScale.BuildingBlocks;
using SwiftScale.Modules.Ordering.Domain.Events;

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
        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            Status = OrderStatus.Pending
        };

        order.Raise(new OrderCreatedDomainEvent(order.Id));

        return order;
    }

    public void MarkAsPaid()
    {
        // Rule: You can only pay for a Pending order
        if (Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException("Order cannot be marked as paid in its current state.");
        }

        Status = OrderStatus.Paid;

        // Optional: Raise a Domain Event if other parts of Ordering need to react 
        // e.g., to notify a warehouse or update an audit log
        Raise(new OrderPaidDomainEvent(Id));
    }

    public void AddItem(Guid productId, decimal price, int quantity)
    {
        _items.Add(new OrderItem(Id, productId, price, quantity));
    }
}
