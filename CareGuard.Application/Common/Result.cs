namespace CareGuard.Application.Common;

public class Result<T> 
{
    public bool IsSuccess { get; init; }
    public T? Value { get; init; }
    public List<string> Errors { get; init; } = [];

    public static Result<T> Success(T value)
    {
        return new Result<T>
        {
            IsSuccess = true,
            Value = value
        };
    }

    public static Result<T> Failure(params string[] errors)
    {
        return new Result<T>
        {
            IsSuccess = false,
            Errors = errors.ToList()
        };
    }
}