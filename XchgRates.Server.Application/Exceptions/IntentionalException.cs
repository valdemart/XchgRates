using System.Net;

namespace XchgRates.Server.Application.Exceptions;

public class IntentionalException(HttpStatusCode statusCode, string message) : Exception(message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
}
