namespace SwiftScale.BuildingBlocks;

    public record Result(bool IsSuccess, string Error = "")
    {
        public static Result Success() => new(true);
        public static Result Failure(string error) => new(false, error);
    }

    public record Result<T>(T? Value, bool IsSuccess, string Error = "") : Result(IsSuccess, Error)
    {
        public static Result<T> Success(T value) => new(value, true);
        public new static Result<T> Failure(string error) => new(default, false, error);
    }

