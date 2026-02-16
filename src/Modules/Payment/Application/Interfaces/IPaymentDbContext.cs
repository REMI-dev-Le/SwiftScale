using Microsoft.EntityFrameworkCore;
using SwiftScale.Modules.Payment.Domain;
using System.Collections.Generic;

namespace SwiftScale.Modules.Payment.Application.Interfaces
{
    public interface IPaymentDbContext
    {
        DbSet<Transaction> Transactions { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
