using TA_API.Models.Requests;
using IValidationService = TA_API.Services.Validation.IValidationService;
using ValidationResult = TA_API.Services.Validation.ValidationResult;

namespace TA_API.Services.Validation;

public class CharacterValidationService : IValidationService
{
    private readonly ILogger<CharacterValidationService> _logger;
    private readonly List<string> _validStatuses = new() { "Alive", "Dead", "Unknown" };

    public CharacterValidationService(ILogger<CharacterValidationService> logger)
    {
        _logger = logger;
    }

    public ValidationResult ValidateCreateRequest(CharacterCreateRequest request)
    {
        var result = new ValidationResult();

        try
        {
            // Validate Name - Required field
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                result.AddError("Character name is required and cannot be empty");
            }
            else if (request.Name.Length < 2)
            {
                result.AddError("Character name must be at least 2 characters long");
            }
            else if (request.Name.Length > 200)
            {
                result.AddError("Character name cannot exceed 200 characters");
            }

            // Validate Status - Required field
            if (string.IsNullOrWhiteSpace(request.Status))
            {
                result.AddError("Character status is required. Valid values: Alive, Dead, Unknown");
            }
            else if (!IsValidStatus(request.Status))
            {
                result.AddError($"Invalid status '{request.Status}'. Valid values are: {string.Join(", ", _validStatuses)}");
            }

            // Validate Species
            if (!string.IsNullOrWhiteSpace(request.Species) && request.Species.Length > 100)
            {
                result.AddError("Species cannot exceed 100 characters");
            }

            // Validate Gender
            if (!string.IsNullOrWhiteSpace(request.Gender))
            {
                var validGenders = new[] { "Male", "Female", "Genderless", "Unknown" };
                if (!validGenders.Contains(request.Gender, StringComparer.OrdinalIgnoreCase))
                {
                    result.AddError($"Invalid gender '{request.Gender}'. Valid values are: {string.Join(", ", validGenders)}");
                }
            }

            // Validate Image URL
            if (!string.IsNullOrWhiteSpace(request.Image) && !IsValidUrl(request.Image))
            {
                result.AddError("Image must be a valid URL");
            }

            // Validate Episode Count
            if (request.EpisodeCount < 0)
            {
                result.AddError("Episode count cannot be negative");
            }
            else if (request.EpisodeCount > 10000)
            {
                result.AddError("Episode count seems invalid (exceeds 10000)");
            }

            if (!result.IsValid)
            {
                _logger.LogWarning("Character creation validation failed with errors: {Errors}", string.Join("; ", result.Errors));
            }
            else
            {
                _logger.LogInformation("Character creation request validated successfully");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating character creation request");
            result.AddError("An error occurred during validation");
            return result;
        }
    }

    public ValidationResult ValidateUpdateRequest(CharacterUpdateRequest request)
    {
        var result = new ValidationResult();

        try
        {
            // Validate Name if provided
            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                if (request.Name.Length < 2)
                {
                    result.AddError("Character name must be at least 2 characters long");
                }
                else if (request.Name.Length > 200)
                {
                    result.AddError("Character name cannot exceed 200 characters");
                }
            }

            // Validate Status if provided
            if (!string.IsNullOrWhiteSpace(request.Status) && !IsValidStatus(request.Status))
            {
                result.AddError($"Invalid status '{request.Status}'. Valid values are: {string.Join(", ", _validStatuses)}");
            }

            // Validate Species if provided
            if (!string.IsNullOrWhiteSpace(request.Species) && request.Species.Length > 100)
            {
                result.AddError("Species cannot exceed 100 characters");
            }

            // Validate Gender if provided
            if (!string.IsNullOrWhiteSpace(request.Gender))
            {
                var validGenders = new[] { "Male", "Female", "Genderless", "Unknown" };
                if (!validGenders.Contains(request.Gender, StringComparer.OrdinalIgnoreCase))
                {
                    result.AddError($"Invalid gender '{request.Gender}'. Valid values are: {string.Join(", ", validGenders)}");
                }
            }

            // Validate Image URL if provided
            if (!string.IsNullOrWhiteSpace(request.Image) && !IsValidUrl(request.Image))
            {
                result.AddError("Image must be a valid URL");
            }

            // Validate Episode Count if provided
            if (request.EpisodeCount < 0)
            {
                result.AddError("Episode count cannot be negative");
            }
            else if (request.EpisodeCount > 10000)
            {
                result.AddError("Episode count seems invalid (exceeds 10000)");
            }

            if (!result.IsValid)
            {
                _logger.LogWarning("Character update validation failed with errors: {Errors}", string.Join("; ", result.Errors));
            }
            else
            {
                _logger.LogInformation("Character update request validated successfully");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating character update request");
            result.AddError("An error occurred during validation");
            return result;
        }
    }

    public bool IsValidStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            return false;
        }

        return _validStatuses.Contains(status, StringComparer.OrdinalIgnoreCase);
    }

    public IEnumerable<string> GetValidStatuses()
    {
        return _validStatuses.AsReadOnly();
    }

    private bool IsValidUrl(string url)
    {
        try
        {
            var uri = new Uri(url);
            return uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;
        }
        catch
        {
            return false;
        }
    }
}
