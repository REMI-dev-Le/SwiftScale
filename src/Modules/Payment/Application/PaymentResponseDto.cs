namespace SwiftScale.Modules.Payment.Application;

// src/Modules/Payment/Application/PaymentResponseDto.cs
public record PaymentResponseDto(Guid TransactionId, bool Success, string Message);