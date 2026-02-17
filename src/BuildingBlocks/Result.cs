using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SwiftScale.BuildingBlocks;

//public record Result(bool IsSuccess, string Error = "")
//{
//    public static Result Success() => new(true);
//    public static Result Failure(string error) => new(false, error);
//}

//public record Result<T>(T? Value, bool IsSuccess, string Error = "") : Result(IsSuccess, Error)
//{
//    public static Result<T> Success(T value) => new(value, true);
//    public new static Result<T> Failure(string error) => new(default, false, error);
//}

public class Error
{
    public static readonly Error None = new Error(string.Empty);
    public string Message { get; }

    public Error(string message)
    {
        Message = message;
    }
}

public class Result<TValue>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public TValue Value { get; }
    public Error Error { get; }

    protected internal Result(TValue value, bool isSuccess, Error error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static Result<TValue> Success(TValue value) => new(value, true, Error.None);
    public static Result<TValue> Failure(Error error) => new(default!, false, error);
}
