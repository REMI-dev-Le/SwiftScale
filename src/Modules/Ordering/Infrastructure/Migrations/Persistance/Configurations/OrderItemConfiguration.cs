using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiftScale.Modules.Ordering.Domain;

namespace SwiftScale.Modules.Ordering.Infrastructure.Migrations.Persistance.Configurations
{
    internal sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItems", "ordering");
            builder.HasKey(oi => oi.Id);

            builder.Property(oi => oi.UnitPrice).HasPrecision(18, 2);
        }
    }
}
