using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace XchgRates.Server.Models;

public class ApiResponse
{
    public bool Success { get; init; }
    public object? Result { get; init; }
    public string? Error { get; init; }

    private ApiResponse() { }

    public static ObjectResult ForSuccess(object? result, HttpStatusCode statusCode = HttpStatusCode.OK)
        => new(
            new ApiResponse
            {
                Success = true,
                Result = result,
                Error = null,
            })
        {
            StatusCode = (int)statusCode,
        };

    public static ObjectResult ForError(string error, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        => new(
            new ApiResponse
            {
                Success = false,
                Result = null,
                Error = error,
            })
        {
            StatusCode = (int)statusCode,
        };
}
