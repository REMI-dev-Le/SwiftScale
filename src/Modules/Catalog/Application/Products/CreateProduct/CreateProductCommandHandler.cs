using MediatR;
using Microsoft.Extensions.Logging;
using SwiftScale.BuildingBlocks;
using SwiftScale.BuildingBlocks.Auth;
using SwiftScale.Modules.Catalog.Application.Interfaces;
using SwiftScale.Modules.Catalog.Domain;

namespace SwiftScale.Modules.Catalog.Application.Products.CreateProduct
{
    public sealed class CreateProductCommandHandler(ICatalogDbContext context, ICurrentUserProvider currentUser, ILogger<CreateProductCommand> logger) : IRequestHandler<CreateProductCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Attempting to create product by User: {UserId}", currentUser.UserId);
            }
            // 1. Create the Money Value Object
            var priceResult = Money.Create(request.Price);
            if (priceResult.IsFailure)
            {
                return Result<Guid>.Failure(priceResult.Error);
            }

            var skuResult = Sku.Create(request.SkuString);
            if (skuResult.IsFailure)
            {
                return Result<Guid>.Failure(skuResult.Error);
            }

            // 2. Attempt to create the Product entity
            var productResult = Product.Create(
                request.Name,
                request.Description,
                priceResult.Value,
                skuResult.Value);

            if (productResult.IsFailure)
            {
                return Result<Guid>.Failure(productResult.Error);
            }

            // 3. Persist
            context.Products.Add(productResult.Value);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Product {ProductId} successfully created for User {UserId}", productResult.Value.Id, currentUser.UserId);

            return Result<Guid>.Success(productResult.Value.Id);
        }
    }
}
