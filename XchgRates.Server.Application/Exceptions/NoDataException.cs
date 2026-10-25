using System.Net;
using XchgRates.Server.Domain.Models;

namespace XchgRates.Server.Application.Exceptions;

public class NoDataException : IntentionalException
{
    public NoDataException()
        : base(HttpStatusCode.NoContent, $"Brak danych o kursach we wskazanym przedziale")
    { }

    public NoDataException(CurrencyData currency)
        : base(HttpStatusCode.NoContent, $"Brak danych o kursach {currency.Code} we wskazanym przedziale")
    { }
}
