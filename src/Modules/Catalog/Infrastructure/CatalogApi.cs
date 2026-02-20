using Microsoft.EntityFrameworkCore;
using SwiftScale.Modules.Catalog.Application.Interfaces;

namespace SwiftScale.Modules.Catalog.Infrastructure
{
    internal sealed class CatalogApi(CatalogDbContext context) : ICatalogApi
    {
        public async Task<decimal?> GetProductPriceAsync(Guid productId, CancellationToken ct)
        {
            var product = await context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == productId, ct);

            return product?.Price.Amount;
        }
    }
}
