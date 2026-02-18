using MediatR;
using Microsoft.Extensions.Logging;
using SwiftScale.Modules.Catalog.Domain;


namespace SwiftScale.Modules.Catalog.Application.Products.CreateProduct
{
    internal sealed class ProductCreatedDomainEventHandler(ILogger<ProductCreatedDomainEventHandler> logger): INotificationHandler<ProductCreatedDomainEvent>
    {
        public Task Handle(ProductCreatedDomainEvent notification, CancellationToken ct)
        {
            // Architect's Note: Use this for things like cache invalidation or local logging
            logger.LogInformation("Domain Event: Product {ProductId} was created locally.", notification.ProductId);

            return Task.CompletedTask;
        }
    }
}
