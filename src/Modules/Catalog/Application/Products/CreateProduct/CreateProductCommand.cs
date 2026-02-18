using MediatR;
using SwiftScale.BuildingBlocks;

namespace SwiftScale.Modules.Catalog.Application.Products.CreateProduct
{
    public record CreateProductCommand(string Name,
                                       string Description,
                                       decimal Price,
                                       string SkuString) : IRequest<Result<Guid>>;
}
