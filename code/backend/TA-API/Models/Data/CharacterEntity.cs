namespace TA_API.Models.Data;

public class CharacterEntity
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Status { get; set; }
    public string? Species { get; set; }
    public string? Type { get; set; }
    public string? Gender { get; set; }
    public string? OriginName { get; set; }
    public string? LocationName { get; set; }
    public string? Image { get; set; }
    public int EpisodeCount { get; set; }
    public int ExternalId { get; set; }
    public string? ExternalUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
