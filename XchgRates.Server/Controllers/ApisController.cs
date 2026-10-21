using Microsoft.AspNetCore.Mvc;
using XchgRates.Server.Application.RequestHandlers;

namespace XchgRates.Server.Controllers;

[ApiController]
[Route("api")]
public class ApisController(
    IGetSupportedApisRequestHandler getSupportedApisRequestHandler,
    IGetCurrenciesRequestHandler getCurrenciesRequestHandler)
    : ControllerBase
{
    [HttpGet]
    public IActionResult GetApis()
    {
        var result = getSupportedApisRequestHandler.Handle();
        return Ok(result);
    }

    [HttpGet("currencies")]
    public IActionResult GetCurrencies(
        [FromQuery] string apiId,
        CancellationToken cancellationToken = default)
    {
        var currencies = getCurrenciesRequestHandler.Handle(apiId, cancellationToken);
        return Ok(currencies);
    }
}
