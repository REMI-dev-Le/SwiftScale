using MediatR;
using SwiftScale.BuildingBlocks;

namespace SwiftScale.Modules.Ordering.Application.Order.GetOrderHistory
{
    public record GetOrderHistoryQuery() : IRequest<Result<IReadOnlyList<OrderHistoryResponse>>>;

    public record OrderHistoryResponse(Guid OrderId,
                                        DateTime CreatedAtUtc,
                                        int Status,
                                        decimal TotalAmount,
                                        int ItemCount);
}
