using SwiftScale.BuildingBlocks;
namespace SwiftScale.Modules.Catalog.Domain
{
    public class Product
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public Money Price { get; private set; } = default!;
        public int Sku { get; private set; } // Stock Keeping Unit

        private Product() { } // EF Core requirement

        public static Result<Product> Create(string name, string description, Money price, int sku)
        {
            // Internal Domain Validation (Invariants)
            if (string.IsNullOrWhiteSpace(name))
            {
                return Result<Product>.Failure(new Error("Product name is required."));
            }

            if (sku <= 0)
            {
                return Result<Product>.Failure(new Error("SKU must be greater than zero."));
            }

            return Result<Product>.Success(new Product
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = description,
                Price = price,
                Sku = sku
            });
        }
    }
}
