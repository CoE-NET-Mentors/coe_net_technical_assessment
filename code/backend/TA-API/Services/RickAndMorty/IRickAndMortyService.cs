using TA_API.Models.RickAndMorty;

namespace TA_API.Services.RickAndMorty;

public interface IRickAndMortyService
{
    /// <summary>
    /// Gets all characters with optional pagination.
    /// </summary>
    /// <param name="page">Page number for pagination (optional)</param>
    /// <returns>A CharacterResponse containing all characters</returns>
    Task<CharacterResponse?> GetAllCharactersAsync(int? page = null);

    /// <summary>
    /// Gets a specific character by ID.
    /// </summary>
    /// <param name="characterId">The ID of the character</param>
    /// <returns>A Character object</returns>
    Task<Character?> GetCharacterByIdAsync(int characterId);

    /// <summary>
    /// Gets multiple characters by their IDs.
    /// </summary>
    /// <param name="characterIds">Array of character IDs</param>
    /// <returns>An array of Character objects</returns>
    Task<Character[]?> GetCharactersByIdsAsync(int[] characterIds);
}
