namespace GymTracker.UserService.Models;

public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T? Data { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;
    public List<string> Errors { get; private set; } = new();

    private Result() { }

    public static Result<T> Success(T data)
    {
        return new Result<T> { IsSuccess = true, Data = data };
    }

    public static Result<T> Failure(string errorMessage)
    {
        return new Result<T> { IsSuccess = false, ErrorMessage = errorMessage };
    }

    public static Result<T> Failure(List<string> errors)
    {
        return new Result<T> { IsSuccess = false, Errors = errors };
    }
}

public class Result
{
    public bool IsSuccess { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;

    private Result() { }

    public static Result Success() => new() { IsSuccess = true };
    public static Result Failure(string errorMessage) => new() { IsSuccess = false, ErrorMessage = errorMessage };
}