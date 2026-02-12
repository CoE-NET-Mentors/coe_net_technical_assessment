using TA_API.Models.Data;
using TA_API.Models.Requests;

namespace TA_API.Services.Characters;

public interface ICharacterService
{
    /// <summary>
    /// Gets all stored characters.
    /// </summary>
    Task<List<CharacterEntity>> GetAllCharactersAsync();

    /// <summary>
    /// Gets a character by ID.
    /// </summary>
    Task<CharacterEntity?> GetCharacterByIdAsync(int id);

    /// <summary>
    /// Creates a new character.
    /// </summary>
    Task<CharacterEntity> CreateCharacterAsync(CharacterCreateRequest request);

    /// <summary>
    /// Updates an existing character.
    /// </summary>
    Task<CharacterEntity?> UpdateCharacterAsync(int id, CharacterUpdateRequest request);

    /// <summary>
    /// Deletes a character.
    /// </summary>
    Task<bool> DeleteCharacterAsync(int id);

    /// <summary>
    /// Imports a character from Rick and Morty API.
    /// </summary>
    Task<CharacterEntity?> ImportCharacterFromApiAsync(int externalCharacterId);

    /// <summary>
    /// Checks if a character exists.
    /// </summary>
    Task<bool> CharacterExistsAsync(int id);
}
