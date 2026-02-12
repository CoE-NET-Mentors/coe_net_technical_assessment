using TA_API.Services.Authentication;

namespace TA_API.Middleware;

public class ApiKeyAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiKeyAuthenticationMiddleware> _logger;

    public ApiKeyAuthenticationMiddleware(RequestDelegate next, ILogger<ApiKeyAuthenticationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IAuthenticationService authService)
    {
        // Skip authentication for login endpoint, health checks, runtime-config and public read-only endpoints
        if (context.Request.Path.StartsWithSegments("/auth/login") ||
            context.Request.Path.StartsWithSegments("/auth/swagger") ||
            context.Request.Path.StartsWithSegments("/lbhealth") ||
            context.Request.Path.StartsWithSegments("/runtime-config") ||
            context.Request.Path.StartsWithSegments("/public") ||
            context.Request.Path.Value == "/")
        {
            await _next(context);
            return;
        }

        // Check for API Key in header
        var apiKeyHeader = context.Request.Headers["X-API-Key"].ToString();
        var authHeader = context.Request.Headers["Authorization"].ToString();

        if (!string.IsNullOrWhiteSpace(apiKeyHeader))
        {
            // Validate API Key
            if (authService.ValidateApiKey(apiKeyHeader))
            {
                _logger.LogInformation("Request authorized with API Key");
                await _next(context);
                return;
            }
            else
            {
                _logger.LogWarning("Invalid API Key provided");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { message = "Invalid API Key" });
                return;
            }
        }

        // Check for Bearer Token in Authorization header
        if (!string.IsNullOrWhiteSpace(authHeader) && authHeader.StartsWith("Bearer "))
        {
            var token = authHeader.Substring("Bearer ".Length).Trim();
            if (authService.ValidateToken(token))
            {
                _logger.LogInformation("Request authorized with Bearer Token");
                context.Items["Username"] = authService.GetUsernameFromToken(token);
                await _next(context);
                return;
            }
            else
            {
                _logger.LogWarning("Invalid Bearer Token provided");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { message = "Invalid or expired token" });
                return;
            }
        }

        _logger.LogWarning("Request attempted without authentication");
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsJsonAsync(new { message = "Missing API Key or Authorization header" });
    }
}
