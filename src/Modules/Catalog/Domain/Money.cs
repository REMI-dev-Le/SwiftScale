using SwiftScale.BuildingBlocks;

namespace SwiftScale.Modules.Catalog.Domain
{
    public record Money(decimal Amount, string Currency)
    {
        public static Result<Money> Create(decimal amount, string currency = "INR")
        {
            if (amount < 0)
            {
                return Result<Money>.Failure(new Error("Amount cannot be negative."));
            }
            return Result<Money>.Success(new Money(amount, currency));
        }
    }
}
