using Microsoft.EntityFrameworkCore;
using SwiftScale.BuildingBlocks.Messaging;

namespace SwiftScale.Modules.Payment.Application.Interfaces
{
    public interface IPaymentDbContext
    {
        DbSet<Domain.Payment> Payments { get; }
        DbSet<InboxMessage> InboxMessages { get; }
        DbSet<OutboxMessage> OutboxMessages { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
