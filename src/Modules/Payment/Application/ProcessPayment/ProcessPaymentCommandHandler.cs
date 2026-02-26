using MediatR;
using Microsoft.Extensions.Logging;
using SwiftScale.BuildingBlocks;
using SwiftScale.BuildingBlocks.Auth;

namespace SwiftScale.Modules.Payment.Application.ProcessPayment
{
    internal sealed class ProcessPaymentCommandHandler(IPublisher publisher, ICurrentUserProvider currentUser, ILogger<ProcessPaymentCommand> logger) : IRequestHandler<ProcessPaymentCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(ProcessPaymentCommand request, CancellationToken ct)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Payment attempt from User: {UserId}", currentUser.UserId);
            }
            // Architect's Note: In a real app, you'd integrate Stripe/Razorpay here
            var paymentId = Guid.NewGuid();

            // Broadcast to the whole system that payment is done
            await publisher.Publish(new PaymentCompletedIntegrationEvent(request.OrderId, paymentId, request.Amount, DateTime.UtcNow), ct);


            logger.LogInformation("Payment successful for {OrderId} from User {UserId}", request.OrderId, currentUser.UserId);

            return Result<Guid>.Success(paymentId);
        }
    }
}
