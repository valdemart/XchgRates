using System.Net;

namespace XchgRates.Server.Application.Exceptions;

public class UnsupportedTargetCurrencyException() : IntentionalException(HttpStatusCode.BadRequest, "Wskazana waluta źródłowa nie jest obsługiwana przez to API");
