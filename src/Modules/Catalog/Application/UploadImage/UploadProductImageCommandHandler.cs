using MediatR;
using Microsoft.Extensions.Logging;
using SwiftScale.BuildingBlocks;
using SwiftScale.BuildingBlocks.Auth;
using SwiftScale.BuildingBlocks.Storage;
using SwiftScale.Modules.Catalog.Application.Interfaces;
using SwiftScale.Modules.Catalog.Application.Products.CreateProduct;

namespace SwiftScale.Modules.Catalog.Application.UploadImage
{
    internal sealed class UploadProductImageCommandHandler(ICatalogDbContext context, IFileStorageService storageService, ICurrentUserProvider currentUser, ILogger<UploadProductImageCommand> logger)
          : IRequestHandler<UploadProductImageCommand, Result<string>>
    {
        public async Task<Result<string>> Handle(UploadProductImageCommand request, CancellationToken ct)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Attempting to upload product image by User: {UserId}", currentUser.UserId);
            }
            // 1. Find the product
            var product = await context.Products.FindAsync([request.ProductId], ct);
            if (product is null)
            {
                return Result<string>.Failure(new Error("Product not found."));
            }

            // 2. Upload the file using our abstraction
            var imagePath = await storageService.UploadAsync(request.FileStream, request.FileName, ct);

            // 3. Update the domain entity
            product.UpdateImagePath(imagePath);

            // 4. Persist change
            await context.SaveChangesAsync(ct);

            logger.LogInformation("Product {ProductId} image successfully uploaded by User {UserId}", product.Id, currentUser.UserId);

            return Result<string>.Success(imagePath);
        }
    }
}
