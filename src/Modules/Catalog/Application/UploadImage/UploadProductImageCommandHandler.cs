using MediatR;
using SwiftScale.BuildingBlocks;
using SwiftScale.BuildingBlocks.Storage;
using SwiftScale.Modules.Catalog.Application.Interfaces;

namespace SwiftScale.Modules.Catalog.Application.UploadImage
{
    internal sealed class UploadProductImageCommandHandler(ICatalogDbContext context, IFileStorageService storageService)
          : IRequestHandler<UploadProductImageCommand, Result<string>>
    {
        public async Task<Result<string>> Handle(UploadProductImageCommand request, CancellationToken ct)
        {
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

            return Result<string>.Success(imagePath);
        }
    }
}
