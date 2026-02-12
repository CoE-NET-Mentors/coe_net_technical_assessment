using System.ComponentModel.DataAnnotations;
using TA_API.Models.Requests;

namespace TA_API.Services.Validation;

public interface IValidationService
{
    /// <summary>
    /// Validates a character creation request.
    /// </summary>
    ValidationResult ValidateCreateRequest(CharacterCreateRequest request);

    /// <summary>
    /// Validates a character update request.
    /// </summary>
    ValidationResult ValidateUpdateRequest(CharacterUpdateRequest request);

    /// <summary>
    /// Validates character status (Alive, Dead, Unknown).
    /// </summary>
    bool IsValidStatus(string? status);

    /// <summary>
    /// Gets all valid status values.
    /// </summary>
    IEnumerable<string> GetValidStatuses();
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();

    public ValidationResult(bool isValid = true)
    {
        IsValid = isValid;
    }

    public ValidationResult(params string[] errors)
    {
        IsValid = errors.Length == 0;
        Errors = errors.ToList();
    }

    public void AddError(string error)
    {
        Errors.Add(error);
        IsValid = false;
    }
}
