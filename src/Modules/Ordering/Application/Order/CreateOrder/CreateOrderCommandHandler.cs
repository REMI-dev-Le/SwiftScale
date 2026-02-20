using MediatR;
using SwiftScale.BuildingBlocks;
using SwiftScale.Modules.Ordering.Application.Interfaces;
using SwiftScale.Modules.Catalog.Application.Interfaces;

namespace SwiftScale.Modules.Catalog.Application.Order.CreateOrder
{
    internal sealed class CreateOrderCommandHandler(IOrderingDbContext context, ICatalogApi catalogApi) // Inject the Internal API
                                                   : IRequestHandler<CreateOrderCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateOrderCommand request, CancellationToken ct)
        {
            // 1. Initialize the Aggregate Root
            var order = SwiftScale.Modules.Ordering.Domain.Order.Create(request.CustomerId); // <-- Fully qualify to avoid ambiguity

            foreach (var itemRequest in request.Items)
            {
                // 2. Verify product existence and price via the Catalog Module
                var unitPrice = await catalogApi.GetProductPriceAsync(itemRequest.ProductId, ct);

                if (unitPrice is null)
                {
                    return Result<Guid>.Failure(new Error($"Product {itemRequest.ProductId} does not exist."));
                }

                // 3. Add item to the order (Logic is encapsulated in the Entity)
                order.AddItem(itemRequest.ProductId, unitPrice.Value, itemRequest.Quantity);
            }

            // 4. Persist the Order and its Items to the 'ordering' schema
            context.Orders.Add(order);
            await context.SaveChangesAsync(ct);

            return Result<Guid>.Success(order.Id);
        }
    }
}
