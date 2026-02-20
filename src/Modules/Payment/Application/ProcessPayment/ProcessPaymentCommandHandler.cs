using MediatR;
using SwiftScale.BuildingBlocks;

namespace SwiftScale.Modules.Payment.Application.ProcessPayment
{
    internal sealed class ProcessPaymentCommandHandler(IPublisher publisher): IRequestHandler<ProcessPaymentCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(ProcessPaymentCommand request, CancellationToken ct)
        {
            // Architect's Note: In a real app, you'd integrate Stripe/Razorpay here
            var paymentId = Guid.NewGuid();

            // Broadcast to the whole system that payment is done
            await publisher.Publish(new PaymentCompletedIntegrationEvent(request.OrderId, paymentId), ct);

            return Result<Guid>.Success(paymentId);
        }
    }
}
