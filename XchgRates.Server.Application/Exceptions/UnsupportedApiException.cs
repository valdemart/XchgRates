using System.Net;

namespace XchgRates.Server.Application.Exceptions;

public class UnsupportedApiException() : IntentionalException(HttpStatusCode.BadRequest, "Wskazane API nie jest obsługiwane");
