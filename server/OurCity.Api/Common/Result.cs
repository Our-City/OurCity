namespace OurCity.Api.Common;

/// <summary>
/// Adapted from asking ChatGPT for a generic Result class for structured/explicit response handling
/// </summary>
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Data { get; }
    public string? Error { get; }

    private Result(bool isSuccess, T? data, string? error)
    {
        IsSuccess = isSuccess;
        Data = data;
        Error = error;
    }

    public static Result<T> Success(T data) => new(true, data, null);

    public static Result<T> Failure(string errorMessage) => new(false, default, errorMessage);

    public static Result<T> Unauthorized() =>
        new(false, default, "You are not authorized to do this.");
}
