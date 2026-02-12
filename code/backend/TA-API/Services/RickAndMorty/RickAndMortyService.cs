using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using TA_API.Models.RickAndMorty;

namespace TA_API.Services.RickAndMorty;

public class RickAndMortyService : IRickAndMortyService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<RickAndMortyService> _logger;
    private const string BaseUrl = "https://rickandmortyapi.com/api";

    // Cache JsonSerializerOptions to avoid recreation on each call
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public RickAndMortyService(HttpClient httpClient, ILogger<RickAndMortyService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<CharacterResponse?> GetAllCharactersAsync(int? page = null)
    {
        try
        {
            // Use QueryHelpers for safe URL query parameter construction
            var baseUrl = $"{BaseUrl}/character";
            var url = page.HasValue
                ? QueryHelpers.AddQueryString(baseUrl, "page", page.ToString()!)
                : baseUrl;

            _logger.LogInformation("Fetching all characters from {Url}", url);
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to fetch characters. Status: {StatusCode}", response.StatusCode);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var characters = JsonSerializer.Deserialize<CharacterResponse>(content, JsonOptions);

            _logger.LogInformation("Successfully fetched characters. Count: {Count}", characters?.Results?.Length ?? 0);
            return characters;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching all characters");
            throw;
        }
    }

    public async Task<Character?> GetCharacterByIdAsync(int characterId)
    {
        try
        {
            var url = $"{BaseUrl}/character/{characterId}";
            _logger.LogInformation("Fetching character with ID {CharacterId} from {Url}", characterId, url);

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Character with ID {CharacterId} not found", characterId);
                    return null;
                }

                _logger.LogError("Failed to fetch character. Status: {StatusCode}", response.StatusCode);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var character = JsonSerializer.Deserialize<Character>(content, JsonOptions);

            _logger.LogInformation("Successfully fetched character: {CharacterName}", character?.Name);
            return character;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching character with ID {CharacterId}", characterId);
            throw;
        }
    }

    public async Task<Character[]?> GetCharactersByIdsAsync(int[] characterIds)
    {
        try
        {
            if (characterIds == null || characterIds.Length == 0)
            {
                _logger.LogWarning("GetCharactersByIdsAsync called with empty or null character IDs");
                return Array.Empty<Character>();
            }

            // Use string.Join efficiently (single operation, not a loop)
            var idString = string.Join(",", characterIds);
            var url = $"{BaseUrl}/character/[{idString}]";
            _logger.LogInformation("Fetching {Count} characters from {Url}", characterIds.Length, url);

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to fetch characters. Status: {StatusCode}", response.StatusCode);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var characters = JsonSerializer.Deserialize<Character[]>(content, JsonOptions);

            _logger.LogInformation("Successfully fetched {Count} characters", characters?.Length ?? 0);
            return characters;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching multiple characters");
            throw;
        }
    }
}
