using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TA_API.Models.Data;
using TA_API.Models.Requests;
using TA_API.Models.RickAndMorty;
using TA_API.Services.Characters;
using TA_API.Services.Data;
using TA_API.Services.RickAndMorty;

namespace TA_API.Tests.Services;

public class CharacterServiceTests
{
    private readonly Mock<IRickAndMortyService> _mockRickAndMortyService;
    private readonly AssessmentDbContext _dbContext;
    private readonly CharacterService _characterService;

    public CharacterServiceTests()
    {
        _mockRickAndMortyService = new Mock<IRickAndMortyService>();

        // Create in-memory database for testing
        var options = new DbContextOptionsBuilder<AssessmentDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;

        _dbContext = new AssessmentDbContext(options);

        var mockLogger = new Mock<ILogger<CharacterService>>();
        _characterService = new CharacterService(_dbContext, _mockRickAndMortyService.Object, mockLogger.Object);
    }

    [Fact]
    public async Task CreateCharacterAsync_ShouldAddCharacterToDatabase()
    {
        // Arrange
        var request = new CharacterCreateRequest
        {
            Name = "Rick Sanchez",
            Status = "Alive",
            Species = "Human",
            Gender = "Male",
            EpisodeCount = 40
        };

        // Act
        var result = await _characterService.CreateCharacterAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Rick Sanchez", result.Name);
        Assert.Equal("Alive", result.Status);
        Assert.True(result.Id > 0);

        // Verify in database
        var character = await _dbContext.Characters.FindAsync(result.Id);
        Assert.NotNull(character);
        Assert.Equal("Rick Sanchez", character.Name);
    }

    [Fact]
    public async Task GetCharacterByIdAsync_ShouldReturnCharacterWhenExists()
    {
        // Arrange
        var character = new CharacterEntity
        {
            Name = "Morty Smith",
            Status = "Alive",
            Species = "Human",
            Gender = "Male",
            EpisodeCount = 40
        };

        _dbContext.Characters.Add(character);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _characterService.GetCharacterByIdAsync(character.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Morty Smith", result.Name);
        Assert.Equal(character.Id, result.Id);
    }

    [Fact]
    public async Task UpdateCharacterAsync_ShouldUpdateCharacterProperties()
    {
        // Arrange
        var character = new CharacterEntity
        {
            Name = "Summer Smith",
            Status = "Alive",
            Species = "Human",
            Gender = "Female",
            EpisodeCount = 30
        };

        _dbContext.Characters.Add(character);
        await _dbContext.SaveChangesAsync();

        var updateRequest = new CharacterUpdateRequest
        {
            Status = "Dead",
            EpisodeCount = 31
        };

        // Act
        var result = await _characterService.UpdateCharacterAsync(character.Id, updateRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Dead", result.Status);
        Assert.Equal(31, result.EpisodeCount);

        // Verify in database
        var updated = await _dbContext.Characters.FindAsync(character.Id);
        Assert.Equal("Dead", updated?.Status);
    }

    [Fact]
    public async Task DeleteCharacterAsync_ShouldRemoveCharacterFromDatabase()
    {
        // Arrange
        var character = new CharacterEntity
        {
            Name = "Beth Smith",
            Status = "Alive",
            Species = "Human",
            Gender = "Female",
            EpisodeCount = 25
        };

        _dbContext.Characters.Add(character);
        await _dbContext.SaveChangesAsync();
        var characterId = character.Id;

        // Act
        var result = await _characterService.DeleteCharacterAsync(characterId);

        // Assert
        Assert.True(result);

        // Verify character is deleted
        var deleted = await _dbContext.Characters.FindAsync(characterId);
        Assert.Null(deleted);
    }
}
