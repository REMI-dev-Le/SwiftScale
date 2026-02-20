using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiftScale.Modules.Ordering.Domain;
namespace SwiftScale.Modules.Ordering.Infrastructure.Migrations.Persistance.Configurations
{
    internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders", "ordering");
            builder.HasKey(o => o.Id);

            // Map the status enum to its integer value in the DB
            builder.Property(o => o.Status).HasConversion<int>();

            // One-to-Many relationship
            builder.HasMany(o => o.Items)
                   .WithOne()
                   .HasForeignKey(oi => oi.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
