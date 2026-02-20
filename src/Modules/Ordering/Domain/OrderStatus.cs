namespace SwiftScale.Modules.Ordering.Domain
{
    public enum OrderStatus
    {
        Pending = 1,    // Created, but payment not confirmed
        Paid = 2,       // Payment successful
        Shipped = 3,    // Out for delivery
        Cancelled = 4,  // Order aborted
        Completed = 5   // Delivered successfully
    }
}
