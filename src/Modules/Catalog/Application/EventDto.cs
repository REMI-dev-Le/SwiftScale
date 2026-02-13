namespace SwiftScale.Modules.Catalog.Application;

// src/Modules/Catalog/Application/EventDto.cs
public record EventDto(Guid Id, string Title, decimal Price, int AvailableSeats);