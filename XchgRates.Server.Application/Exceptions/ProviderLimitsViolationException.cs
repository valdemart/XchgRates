using System.Net;

namespace XchgRates.Server.Application.Exceptions;

public class ProviderLimitsViolationException(string violation) : IntentionalException(HttpStatusCode.BadRequest, violation);
