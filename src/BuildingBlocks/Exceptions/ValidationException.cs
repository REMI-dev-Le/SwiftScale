namespace SwiftScale.BuildingBlocks.Exceptions
{
    public sealed class CustomValidationException(IEnumerable<ValidationError> errors): Exception("Validation failed")
    {
        public IEnumerable<ValidationError> Errors { get; } = errors;
    }

    public record ValidationError(string PropertyName, string ErrorMessage);
}
