using SwiftScale.BuildingBlocks;

namespace SwiftScale.Modules.Catalog.Domain
{
    public record Sku
    {
        private const int DefaultLength = 8;
        public string Value { get; init; }

        private Sku(string value)
        {
            Value = value;
        }

        public static Result<Sku> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length != DefaultLength)
            {
                return Result<Sku>.Failure(new Error($"SKU must be exactly {DefaultLength} characters."));
            }

            return Result<Sku>.Success(new Sku(value));
        }
    }
}
