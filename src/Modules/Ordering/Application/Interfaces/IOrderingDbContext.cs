using Microsoft.EntityFrameworkCore;
using SwiftScale.BuildingBlocks.Messaging;
using SwiftScale.Modules.Ordering.Domain;

namespace SwiftScale.Modules.Ordering.Application.Interfaces
{
    public interface IOrderingDbContext
    {
        DbSet<SwiftScale.Modules.Ordering.Domain.Order> Orders { get; }
        DbSet<OrderItem> OrderItems { get; }
        DbSet<OutboxMessage> OutboxMessages { get; }
        DbSet<InboxMessage> InboxMessages { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
