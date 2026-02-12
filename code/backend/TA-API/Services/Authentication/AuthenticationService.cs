using System.Text;
using TA_API.Models.Requests;
using TA_API.Models.Responses;

namespace TA_API.Services.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthenticationService> _logger;

    // Simple in-memory user store (replace with database in production)
    private readonly Dictionary<string, string> _validUsers = new()
    {
        { "admin", "Admin@123" },
        { "user", "User@123" },
        { "test", "Test@123" }
    };

    public AuthenticationService(IConfiguration configuration, ILogger<AuthenticationService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public bool ValidateApiKey(string? apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            _logger.LogWarning("API key validation failed: Key is empty or null");
            return false;
        }

        var validApiKey = _configuration["Authentication:ApiKey"];
        if (string.IsNullOrWhiteSpace(validApiKey))
        {
            _logger.LogWarning("Valid API key not configured");
            return false;
        }

        var isValid = apiKey.Equals(validApiKey, StringComparison.Ordinal);
        if (!isValid)
        {
            _logger.LogWarning("API key validation failed: Invalid key provided");
        }
        else
        {
            _logger.LogInformation("API key validation successful");
        }

        return isValid;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        return await Task.Run(() =>
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                {
                    _logger.LogWarning("Login failed: Missing username or password");
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Username and password are required"
                    };
                }

                if (!_validUsers.TryGetValue(request.Username, out var storedPassword))
                {
                    _logger.LogWarning("Login failed: User not found - {Username}", request.Username);
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Invalid username or password"
                    };
                }

                if (!storedPassword.Equals(request.Password, StringComparison.Ordinal))
                {
                    _logger.LogWarning("Login failed: Incorrect password - {Username}", request.Username);
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Invalid username or password"
                    };
                }

                var token = GenerateToken(request.Username);
                _logger.LogInformation("User logged in successfully: {Username}", request.Username);

                return new AuthResponse
                {
                    Success = true,
                    Message = "Login successful",
                    Token = token,
                    User = new UserInfo
                    {
                        Username = request.Username,
                        Role = request.Username == "admin" ? "Administrator" : "User"
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login process");
                return new AuthResponse
                {
                    Success = false,
                    Message = "An error occurred during login"
                };
            }
        });
    }

    public string GenerateToken(string username)
    {
        try
        {
            // Simple token generation (replace with JWT in production)
            var tokenData = $"{username}:{DateTime.UtcNow.Ticks}";
            var tokenBytes = Encoding.UTF8.GetBytes(tokenData);
            var token = Convert.ToBase64String(tokenBytes);
            _logger.LogInformation("Token generated for user: {Username}", username);
            return token;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating token for user: {Username}", username);
            throw;
        }
    }

    public bool ValidateToken(string? token)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                _logger.LogWarning("Token validation failed: Token is empty or null");
                return false;
            }

            var decodedBytes = Convert.FromBase64String(token);
            var decodedString = Encoding.UTF8.GetString(decodedBytes);
            var parts = decodedString.Split(':');

            if (parts.Length != 2)
            {
                _logger.LogWarning("Token validation failed: Invalid token format");
                return false;
            }

            _logger.LogInformation("Token validated successfully for user: {Username}", parts[0]);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Token validation failed");
            return false;
        }
    }

    public string? GetUsernameFromToken(string? token)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return null;
            }

            var decodedBytes = Convert.FromBase64String(token);
            var decodedString = Encoding.UTF8.GetString(decodedBytes);
            var parts = decodedString.Split(':');

            return parts.Length == 2 ? parts[0] : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting username from token");
            return null;
        }
    }
}
