using Microsoft.EntityFrameworkCore;
using TA_API.Models.Data;
using TA_API.Models.Requests;
using TA_API.Models.RickAndMorty;
using TA_API.Services.Data;
using TA_API.Services.RickAndMorty;

namespace TA_API.Services.Characters;

public class CharacterService : ICharacterService
{
    private readonly AssessmentDbContext _dbContext;
    private readonly IRickAndMortyService _rickAndMortyService;
    private readonly ILogger<CharacterService> _logger;

    public CharacterService(
        AssessmentDbContext dbContext,
        IRickAndMortyService rickAndMortyService,
        ILogger<CharacterService> logger)
    {
        _dbContext = dbContext;
        _rickAndMortyService = rickAndMortyService;
        _logger = logger;
    }

    public async Task<List<CharacterEntity>> GetAllCharactersAsync()
    {
        try
        {
            _logger.LogInformation("Fetching all characters from database");
            var characters = await _dbContext.Characters.ToListAsync();
            _logger.LogInformation("Retrieved {Count} characters from database", characters.Count);
            return characters;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all characters from database");
            throw;
        }
    }

    public async Task<CharacterEntity?> GetCharacterByIdAsync(int id)
    {
        try
        {
            _logger.LogInformation("Fetching character with ID {CharacterId} from database", id);
            var character = await _dbContext.Characters.FindAsync(id);
            if (character != null)
            {
                _logger.LogInformation("Character found: {CharacterName}", character.Name);
            }
            else
            {
                _logger.LogWarning("Character with ID {CharacterId} not found", id);
            }
            return character;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching character with ID {CharacterId}", id);
            throw;
        }
    }

    public async Task<CharacterEntity> CreateCharacterAsync(CharacterCreateRequest request)
    {
        try
        {
            _logger.LogInformation("Creating new character: {CharacterName}", request.Name);

            var character = new CharacterEntity
            {
                Name = request.Name,
                Status = request.Status,
                Species = request.Species,
                Type = request.Type,
                Gender = request.Gender,
                OriginName = request.OriginName,
                LocationName = request.LocationName,
                Image = request.Image,
                EpisodeCount = request.EpisodeCount,
                ExternalId = request.ExternalId ?? 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _dbContext.Characters.Add(character);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Character created successfully with ID {CharacterId}", character.Id);
            return character;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating character");
            throw;
        }
    }

    public async Task<CharacterEntity?> UpdateCharacterAsync(int id, CharacterUpdateRequest request)
    {
        try
        {
            _logger.LogInformation("Updating character with ID {CharacterId}", id);

            var character = await _dbContext.Characters.FindAsync(id);
            if (character == null)
            {
                _logger.LogWarning("Character with ID {CharacterId} not found for update", id);
                return null;
            }

            character.Name = request.Name ?? character.Name;
            character.Status = request.Status ?? character.Status;
            character.Species = request.Species ?? character.Species;
            character.Type = request.Type ?? character.Type;
            character.Gender = request.Gender ?? character.Gender;
            character.OriginName = request.OriginName ?? character.OriginName;
            character.LocationName = request.LocationName ?? character.LocationName;
            character.Image = request.Image ?? character.Image;
            character.EpisodeCount = request.EpisodeCount > 0 ? request.EpisodeCount : character.EpisodeCount;
            character.UpdatedAt = DateTime.UtcNow;

            _dbContext.Characters.Update(character);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Character with ID {CharacterId} updated successfully", id);
            return character;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating character with ID {CharacterId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteCharacterAsync(int id)
    {
        try
        {
            _logger.LogInformation("Deleting character with ID {CharacterId}", id);

            var character = await _dbContext.Characters.FindAsync(id);
            if (character == null)
            {
                _logger.LogWarning("Character with ID {CharacterId} not found for deletion", id);
                return false;
            }

            _dbContext.Characters.Remove(character);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Character with ID {CharacterId} deleted successfully", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting character with ID {CharacterId}", id);
            throw;
        }
    }

    public async Task<CharacterEntity?> ImportCharacterFromApiAsync(int externalCharacterId)
    {
        try
        {
            _logger.LogInformation("Importing character with external ID {ExternalCharacterId} from Rick and Morty API", externalCharacterId);

            // Check if character already exists
            var existingCharacter = await _dbContext.Characters
                .FirstOrDefaultAsync(c => c.ExternalId == externalCharacterId);
            if (existingCharacter != null)
            {
                _logger.LogWarning("Character with external ID {ExternalCharacterId} already exists in database", externalCharacterId);
                return existingCharacter;
            }

            // Fetch from Rick and Morty API
            var apiCharacter = await _rickAndMortyService.GetCharacterByIdAsync(externalCharacterId);
            if (apiCharacter == null)
            {
                _logger.LogWarning("Character with ID {CharacterId} not found in Rick and Morty API", externalCharacterId);
                return null;
            }

            // Create database entity from API response
            var character = new CharacterEntity
            {
                Name = apiCharacter.Name,
                Status = apiCharacter.Status,
                Species = apiCharacter.Species,
                Type = apiCharacter.Type,
                Gender = apiCharacter.Gender,
                OriginName = apiCharacter.Origin?.Name,
                LocationName = apiCharacter.Location?.Name,
                Image = apiCharacter.Image,
                EpisodeCount = apiCharacter.Episode?.Length ?? 0,
                ExternalId = externalCharacterId,
                ExternalUrl = apiCharacter.Url,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _dbContext.Characters.Add(character);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Character {CharacterName} imported successfully with ID {CharacterId}", character.Name, character.Id);
            return character;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing character from Rick and Morty API with external ID {ExternalCharacterId}", externalCharacterId);
            throw;
        }
    }

    public async Task<bool> CharacterExistsAsync(int id)
    {
        try
        {
            return await _dbContext.Characters.AnyAsync(c => c.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if character with ID {CharacterId} exists", id);
            throw;
        }
    }
}
