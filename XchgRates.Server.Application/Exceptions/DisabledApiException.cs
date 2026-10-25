using System.Net;

namespace XchgRates.Server.Application.Exceptions;

public class DisabledApiException() : IntentionalException(HttpStatusCode.BadRequest, "Wskazane API jest chwilowo niedostępne");
