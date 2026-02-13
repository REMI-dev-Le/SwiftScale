using Microsoft.EntityFrameworkCore;
using SwiftScale.Modules.Identity.Domain;

namespace SwiftScale.Modules.Identity.Application.Interfaces
{
    public interface IIdentityDbContext
    {
        DbSet<User> Users { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
