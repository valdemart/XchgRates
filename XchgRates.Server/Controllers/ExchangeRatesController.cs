using Microsoft.AspNetCore.Mvc;
using XchgRates.Server.Application.RequestHandlers;
using XchgRates.Server.Models;

namespace XchgRates.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExchangeRatesController(
    IGetExchangeRatesRequestHandler getExchangeRatesRequestHandler)
    : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetExchangeRates(
        [FromQuery] string apiId,
        [FromQuery] string sourceCurrencyCode,
        [FromQuery] string targetCurrencyCode,
        [FromQuery] DateOnly? startDate = null,
        [FromQuery] DateOnly? endDate = null,
        CancellationToken cancellationToken = default)
    {
        var result = await getExchangeRatesRequestHandler.Handle(apiId, sourceCurrencyCode, targetCurrencyCode, startDate, endDate, cancellationToken);
        return ApiResponse.ForSuccess(result);
    }
}
