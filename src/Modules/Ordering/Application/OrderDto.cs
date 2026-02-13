namespace SwiftScale.Modules.Ordering.Application;

// src/Modules/Ordering/Application/OrderDto.cs
public record OrderDto(Guid Id, Guid UserId, Guid EventId, string Status, DateTime OrderDate);