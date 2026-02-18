using SwiftScale.BuildingBlocks;
namespace SwiftScale.Modules.Catalog.Domain
{
    public class Product : Entity
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public Money Price { get; private set; } = default!;
        public Sku Sku { get; private set; } = default;// Stock Keeping Unit

        private Product() { } // EF Core requirement

        public static Result<Product> Create(string name, string description, Money price, Sku sku)
        {
            // Internal Domain Validation (Invariants)
            if (string.IsNullOrWhiteSpace(name))
            {
                return Result<Product>.Failure(new Error("Product name is required."));
            }

            if (sku is null || string.IsNullOrWhiteSpace(sku.Value))
            {
                return Result<Product>.Failure(new Error("SKU is required."));
            }

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = description,
                Price = price,
                Sku = sku
            };

            product.RaiseDomainEvent(new ProductCreatedDomainEvent(product.Id));

            return Result<Product>.Success(product);
        }
    }
}
