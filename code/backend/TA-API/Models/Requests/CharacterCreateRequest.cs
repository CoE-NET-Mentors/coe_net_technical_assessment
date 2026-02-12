using System.ComponentModel.DataAnnotations;

namespace TA_API.Models.Requests;

public class CharacterCreateRequest
{
    [Required(ErrorMessage = "Character name is required")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 200 characters")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Character status is required. Valid values: Alive, Dead, Unknown")]
    [RegularExpression(@"^(Alive|Dead|Unknown)$", ErrorMessage = "Status must be 'Alive', 'Dead', or 'Unknown'")]
    public string? Status { get; set; }

    [StringLength(100, ErrorMessage = "Species cannot exceed 100 characters")]
    public string? Species { get; set; }

    [StringLength(200, ErrorMessage = "Type cannot exceed 200 characters")]
    public string? Type { get; set; }

    [RegularExpression(@"^(Male|Female|Genderless|Unknown)$", ErrorMessage = "Gender must be 'Male', 'Female', 'Genderless', or 'Unknown'")]
    public string? Gender { get; set; }

    [StringLength(200, ErrorMessage = "Origin name cannot exceed 200 characters")]
    public string? OriginName { get; set; }

    [StringLength(200, ErrorMessage = "Location name cannot exceed 200 characters")]
    public string? LocationName { get; set; }

    [Url(ErrorMessage = "Image must be a valid URL")]
    public string? Image { get; set; }

    [Range(0, 10000, ErrorMessage = "Episode count must be between 0 and 10000")]
    public int EpisodeCount { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "External ID must be a positive integer")]
    public int? ExternalId { get; set; }
}
