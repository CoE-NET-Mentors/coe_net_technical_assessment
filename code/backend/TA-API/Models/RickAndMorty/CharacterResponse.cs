namespace TA_API.Models.RickAndMorty;

public class CharacterResponse
{
    public Info? Info { get; set; }
    public Character[]? Results { get; set; }
}

public class Info
{
    public int Count { get; set; }
    public int Pages { get; set; }
    public string? Next { get; set; }
    public string? Prev { get; set; }
}
