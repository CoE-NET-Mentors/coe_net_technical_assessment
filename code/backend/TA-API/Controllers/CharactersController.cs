using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using TA_API.Models.Requests;
using TA_API.Services.Characters;
using TA_API.Services.RickAndMorty;
using TA_API.Services.Validation;

namespace TA_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CharactersController : ControllerBase
{
    private readonly ICharacterService _characterService;
    private readonly IRickAndMortyService _rickAndMortyService;
    private readonly IValidationService _validationService;
    private readonly ILogger<CharactersController> _logger;

    public CharactersController(
        ICharacterService characterService,
        IRickAndMortyService rickAndMortyService,
        IValidationService validationService,
        ILogger<CharactersController> logger)
    {
        _characterService = characterService;
        _rickAndMortyService = rickAndMortyService;
        _validationService = validationService;
        _logger = logger;
    }

    #region CRUD Operations

    /// <summary>
    /// Gets all stored characters from the local database.
    /// </summary>
    /// <returns>A list of all characters</returns>
    /// <response code="200">Successfully retrieved all characters</response>
    /// <response code="500">Internal server error</response>
    [HttpGet]
    public async Task<IActionResult> GetAllCharacters()
    {
        try
        {
            var characters = await _characterService.GetAllCharactersAsync();
            return Ok(new { count = characters.Count, data = characters });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all characters");
            return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets a specific character by ID from the local database.
    /// </summary>
    /// <param name="id">The ID of the character</param>
    /// <returns>A character object</returns>
    /// <response code="200">Successfully retrieved the character</response>
    /// <response code="404">Character not found</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCharacterById(int id)
    {
        try
        {
            var character = await _characterService.GetCharacterByIdAsync(id);
            if (character == null)
            {
                return NotFound(new { message = $"Character with ID {id} not found" });
            }

            return Ok(character);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving character with ID {CharacterId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Creates a new character in the local database.
    /// </summary>
    /// <param name="request">The character data to create</param>
    /// <returns>The created character</returns>
    /// <response code="201">Character created successfully</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="500">Internal server error</response>
    [HttpPost]
    public async Task<IActionResult> CreateCharacter([FromBody] CharacterCreateRequest request)
    {
        try
        {
            // Validate request using custom validation service
            var validationResult = _validationService.ValidateCreateRequest(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new { message = "Validation failed", errors = validationResult.Errors });
            }

            var character = await _characterService.CreateCharacterAsync(request);
            return CreatedAtAction(nameof(GetCharacterById), new { id = character.Id }, character);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating character");
            return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Updates an existing character in the local database.
    /// </summary>
    /// <param name="id">The ID of the character to update</param>
    /// <param name="request">The character data to update</param>
    /// <returns>The updated character</returns>
    /// <response code="200">Character updated successfully</response>
    /// <response code="404">Character not found</response>
    /// <response code="500">Internal server error</response>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCharacter(int id, [FromBody] CharacterUpdateRequest request)
    {
        try
        {
            // Validate request using custom validation service
            var validationResult = _validationService.ValidateUpdateRequest(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new { message = "Validation failed", errors = validationResult.Errors });
            }

            var character = await _characterService.UpdateCharacterAsync(id, request);
            if (character == null)
            {
                return NotFound(new { message = $"Character with ID {id} not found" });
            }

            return Ok(character);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating character with ID {CharacterId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Deletes a character from the local database.
    /// </summary>
    /// <param name="id">The ID of the character to delete</param>
    /// <returns>No content</returns>
    /// <response code="204">Character deleted successfully</response>
    /// <response code="404">Character not found</response>
    /// <response code="500">Internal server error</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCharacter(int id)
    {
        try
        {
            var deleted = await _characterService.DeleteCharacterAsync(id);
            if (!deleted)
            {
                return NotFound(new { message = $"Character with ID {id} not found" });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting character with ID {CharacterId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
        }
    }

    #endregion

    #region Rick and Morty API Operations

    /// <summary>
    /// Gets all characters from the Rick and Morty API with optional pagination.
    /// </summary>
    /// <param name="page">Page number for pagination (optional)</param>
    /// <returns>A list of all characters from the API</returns>
    /// <response code="200">Successfully retrieved all characters</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("api/all")]
    public async Task<IActionResult> GetAllCharactersFromApi([FromQuery] int? page = null)
    {
        try
        {
            var result = await _rickAndMortyService.GetAllCharactersAsync(page);
            if (result == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to fetch characters from Rick and Morty API");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all characters from API");
            return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets a specific character from the Rick and Morty API by external ID.
    /// </summary>
    /// <param name="externalId">The external ID of the character</param>
    /// <returns>A character object</returns>
    /// <response code="200">Successfully retrieved the character</response>
    /// <response code="404">Character not found</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("api/{externalId}")]
    public async Task<IActionResult> GetCharacterFromApi(int externalId)
    {
        try
        {
            var result = await _rickAndMortyService.GetCharacterByIdAsync(externalId);
            if (result == null)
            {
                return NotFound(new { message = $"Character with ID {externalId} not found" });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving character from API with ID {ExternalId}", externalId);
            return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Imports a character from the Rick and Morty API to the local database.
    /// </summary>
    /// <param name="externalId">The external ID of the character to import</param>
    /// <returns>The imported character</returns>
    /// <response code="201">Character imported successfully</response>
    /// <response code="404">Character not found</response>
    /// <response code="409">Character already exists in database</response>
    /// <response code="500">Internal server error</response>
    [HttpPost("import/{externalId}")]
    public async Task<IActionResult> ImportCharacterFromApi(int externalId)
    {
        try
        {
            var character = await _characterService.ImportCharacterFromApiAsync(externalId);
            if (character == null)
            {
                return NotFound(new { message = $"Character with ID {externalId} not found in Rick and Morty API" });
            }

            return CreatedAtAction(nameof(GetCharacterById), new { id = character.Id }, character);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing character from API with external ID {ExternalId}", externalId);
            return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
        }
    }

    #endregion
}
