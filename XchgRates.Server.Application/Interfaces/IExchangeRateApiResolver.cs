using XchgRates.Server.Domain.Interfaces;
using XchgRates.Server.Domain.Models;

namespace XchgRates.Server.Application.Interfaces;

public interface IExchangeRateApiResolver
{
    ApiConfiguration[] GetSupportedApis();
    IExchangeRateProvider? Resolve(string apiId, out ApiConfiguration? apiConfiguration);
}
