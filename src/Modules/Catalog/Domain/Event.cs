namespace SwiftScale.Modules.Catalog.Domain;

// src/Modules/Catalog/Domain/Event.cs
public class Event
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int AvailableSeats { get; private set; }

    public static Event Create(string title, decimal price, int seats)
    {
        return new Event
        {
            Id = Guid.NewGuid(),
            Title = title,
            Price = price,
            AvailableSeats = seats
        };
    }
}