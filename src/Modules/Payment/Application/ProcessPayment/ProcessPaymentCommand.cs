using MediatR;
using SwiftScale.BuildingBlocks;

namespace SwiftScale.Modules.Payment.Application.ProcessPayment
{
    public record ProcessPaymentCommand(Guid OrderId, decimal Amount) : IRequest<Result<Guid>>;

    public enum PaymentStatus
    {
        Pending = 1,
        Completed = 2,
        Failed = 3
    }
}
