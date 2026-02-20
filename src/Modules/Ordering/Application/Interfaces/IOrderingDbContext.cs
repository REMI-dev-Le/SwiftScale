using Microsoft.EntityFrameworkCore;
using SwiftScale.Modules.Ordering.Domain;
using System.Collections.Generic;

namespace SwiftScale.Modules.Ordering.Application.Interfaces
{
    public interface IOrderingDbContext
    {
        DbSet<SwiftScale.Modules.Ordering.Domain.Order> Orders { get; }
        DbSet<OrderItem> OrderItems { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
