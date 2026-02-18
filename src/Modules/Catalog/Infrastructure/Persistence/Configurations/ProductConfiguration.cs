using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiftScale.Modules.Catalog.Domain;

namespace SwiftScale.Modules.Catalog.Infrastructure.Persistence.Configurations
{
    internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products", "catalog");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
            builder.Property(p => p.Description).HasMaxLength(1000);

            // Architect Rule: Map the Value Object to columns
            builder.OwnsOne(p => p.Price, priceBuilder =>
            {
                priceBuilder.Property(m => m.Amount).HasColumnName("PriceAmount").HasPrecision(18, 2);
                priceBuilder.Property(m => m.Currency).HasColumnName("PriceCurrency").HasMaxLength(3);
            });

             builder.Property(p => p.Sku)
                               .HasConversion(sku => sku.Value, value => Sku.Create(value).Value) // Map Object <-> String
                               .HasColumnName("Sku")
                               .IsRequired();

            builder.Property(p => p.ImagePath).HasMaxLength(500);
        }
    }
}
