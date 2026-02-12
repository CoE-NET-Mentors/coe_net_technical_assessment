using TA_API.Models.Requests;
using TA_API.Models.Responses;

namespace TA_API.Services.Authentication;

public interface IAuthenticationService
{
    /// <summary>
    /// Validates API key from header.
    /// </summary>
    bool ValidateApiKey(string? apiKey);

    /// <summary>
    /// Authenticates user with username and password.
    /// </summary>
    Task<AuthResponse> LoginAsync(LoginRequest request);

    /// <summary>
    /// Generates an authentication token.
    /// </summary>
    string GenerateToken(string username);

    /// <summary>
    /// Validates an authentication token.
    /// </summary>
    bool ValidateToken(string? token);

    /// <summary>
    /// Gets the username from token.
    /// </summary>
    string? GetUsernameFromToken(string? token);
}
