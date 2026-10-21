namespace XchgRates.Server.Domain.Models;

public sealed record ExchangeRate(CurrencyData Currency, DateOnly Date, decimal Rate);
