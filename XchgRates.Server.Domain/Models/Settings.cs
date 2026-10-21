namespace XchgRates.Server.Domain.Models;

public class Settings
{
    public ApiConfiguration[] ApiConfigurations { get; set; } = [];
    public string DateFormat { get; set; } = "yyyy-MM-dd";
    public CurrencyOrderings SourceCurrencyOrdering { get; set; } = CurrencyOrderings.CodeAscending;
    public CurrencyOrderings TargetCurrencyOrdering { get; set; } = CurrencyOrderings.CodeAscending;
}

public class ApiConfiguration
{
    public required string Id { get; set; }
    public required string Provider { get; set; }
    public bool Enabled { get; set; } = false;
    public required string DisplayName { get; set; }
    public required string BaseUrl { get; set; }
}

public enum CurrencyOrderings
{
    CodeAscending,
    CodeDescending,
    NameAscending,
    NameDescending,
}
