namespace SwiftScale.Modules.Catalog.Application.Interfaces
{
    public interface ICatalogApi
    {
        Task<decimal?> GetProductPriceAsync(Guid productId, CancellationToken ct = default);
    }
}
