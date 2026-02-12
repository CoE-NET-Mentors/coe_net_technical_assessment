using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using TA_API.Models.Requests;
using TA_API.Services.Authentication;

namespace TA_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthenticationService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Authenticates user with username and password, returns authentication token.
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>Authentication token and user info</returns>
    /// <response code="200">Login successful</response>
    /// <response code="400">Invalid credentials</response>
    /// <response code="500">Internal server error</response>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            if (request == null)
            {
                return BadRequest(new { message = "Request body is required" });
            }

            var result = await _authService.LoginAsync(request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Internal server error: {ex.Message}" });
        }
    }

    /// <summary>
    /// Validates an authentication token.
    /// </summary>
    /// <param name="token">Token to validate</param>
    /// <returns>Validation result</returns>
    /// <response code="200">Token is valid</response>
    /// <response code="401">Token is invalid</response>
    [HttpGet("validate")]
    public IActionResult ValidateToken([FromHeader(Name = "Authorization")] string token)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return Unauthorized(new { message = "Token is required" });
            }

            var bearerToken = token.StartsWith("Bearer ") ? token.Substring("Bearer ".Length).Trim() : token;

            if (!_authService.ValidateToken(bearerToken))
            {
                return Unauthorized(new { message = "Invalid or expired token" });
            }

            var username = _authService.GetUsernameFromToken(bearerToken);
            return Ok(new { valid = true, username });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating token");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Internal server error: {ex.Message}" });
        }
    }
}
