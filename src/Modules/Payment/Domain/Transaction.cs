namespace SwiftScale.Modules.Payment.Domain;

public class Transaction
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public decimal Amount { get; private set; }
    public bool IsSuccess { get; private set; }

    public static Transaction Create(Guid orderId, decimal amount)
    {
        return new Transaction
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            Amount = amount,
            IsSuccess = false
        };
    }
}
