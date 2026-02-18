using MediatR;
using SwiftScale.BuildingBlocks;

namespace SwiftScale.Modules.Catalog.Application.Products.GetProduct
{
    public record ProductResponse(Guid Id, string Name, string Description, decimal Price, string Sku);
    public record GetProductQuery(Guid Id) : IRequest<Result<ProductResponse>>;
}
