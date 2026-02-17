using Microsoft.EntityFrameworkCore;
using SwiftScale.Modules.Catalog.Domain;

namespace SwiftScale.Modules.Catalog.Application.Interfaces
{
    public interface ICatalogDbContext
    {
        DbSet<Event> Transactions { get; } 
        DbSet<Product> Products { get; } 
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
