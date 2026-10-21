namespace XchgRates.Server.Models;

public record ApiResponse<T>(bool Success, T? Result, string? Error) : ApiResponse(Success, Error);

public record ApiResponse(bool Success, string? Error)
{
    public static ApiResponse<T> ForSuccess<T>(T? result)
        => new(true, result, null);

    public static ApiResponse ForError(string error)
        => new(false, error);
}
