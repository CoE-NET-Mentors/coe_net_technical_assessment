using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace TA_API.Controllers;

[ApiController]
[Route("/runtime-config")]
public class RuntimeConfigController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public RuntimeConfigController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult Get()
    {
        // Build public apiUrl based on current request host to avoid hardcoding
        var scheme = Request.Scheme;
        var host = Request.Host.Value;
        var apiUrl = $"{scheme}://{host}/public/characters";

        // Do NOT expose API keys here. Only provide non-secret runtime settings.
        return Ok(new { apiUrl });
    }
}
