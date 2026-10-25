using Microsoft.AspNetCore.Mvc;
using XchgRates.Server.Application.RequestHandlers;
using XchgRates.Server.Models;

namespace XchgRates.Server.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class SettingsController(
    IGetSettingsRequestHandler getSettingsRequestHandler)
    : ControllerBase
{
    [HttpGet]
    public IActionResult GetSettings()
    {
        var result = getSettingsRequestHandler.Handle();
        return ApiResponse.ForSuccess(result);
    }
}
