using MediatR;
using SwiftScale.BuildingBlocks;

namespace SwiftScale.Modules.Catalog.Application.Order.CreateOrder
{
    public record OrderItemRequest(Guid ProductId, int Quantity);

    public record CreateOrderCommand(Guid CustomerId, List<OrderItemRequest> Items) : IRequest<Result<Guid>>;
}
