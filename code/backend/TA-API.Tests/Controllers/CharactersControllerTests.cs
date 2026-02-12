using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TA_API.Controllers;
using TA_API.Models.Data;
using TA_API.Models.Requests;
using TA_API.Services.Characters;
using TA_API.Services.RickAndMorty;
using TA_API.Services.Validation;

namespace TA_API.Tests.Controllers;

public class CharactersControllerTests
{
    private readonly Mock<ICharacterService> _mockCharacterService;
    private readonly Mock<IRickAndMortyService> _mockRickAndMortyService;
    private readonly Mock<IValidationService> _mockValidationService;
    private readonly CharactersController _controller;

    public CharactersControllerTests()
    {
        _mockCharacterService = new Mock<ICharacterService>();
        _mockRickAndMortyService = new Mock<IRickAndMortyService>();
        _mockValidationService = new Mock<IValidationService>();

        var mockLogger = new Mock<ILogger<CharactersController>>();

        _controller = new CharactersController(
            _mockCharacterService.Object,
            _mockRickAndMortyService.Object,
            _mockValidationService.Object,
            mockLogger.Object);
    }

    [Fact]
    public async Task GetAllCharacters_ShouldReturnOkWithCharacterList()
    {
        // Arrange
        var characters = new List<CharacterEntity>
        {
            new() { Id = 1, Name = "Rick", Status = "Alive" },
            new() { Id = 2, Name = "Morty", Status = "Alive" }
        };

        _mockCharacterService.Setup(s => s.GetAllCharactersAsync())
            .ReturnsAsync(characters);

        // Act
        var result = await _controller.GetAllCharacters();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        _mockCharacterService.Verify(s => s.GetAllCharactersAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateCharacter_WithValidRequest_ShouldReturnCreatedAtAction()
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

        var createdCharacter = new CharacterEntity
        {
            Id = 1,
            Name = request.Name,
            Status = request.Status
        };

        var validationResult = new ValidationResult(true);
        _mockValidationService.Setup(v => v.ValidateCreateRequest(request))
            .Returns(validationResult);

        _mockCharacterService.Setup(s => s.CreateCharacterAsync(request))
            .ReturnsAsync(createdCharacter);

        // Act
        var result = await _controller.CreateCharacter(request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(CharactersController.GetCharacterById), createdResult.ActionName);
        _mockCharacterService.Verify(s => s.CreateCharacterAsync(request), Times.Once);
    }

    [Fact]
    public async Task GetCharacterById_WithValidId_ShouldReturnOkWithCharacter()
    {
        // Arrange
        var characterId = 1;
        var character = new CharacterEntity
        {
            Id = characterId,
            Name = "Rick Sanchez",
            Status = "Alive"
        };

        _mockCharacterService.Setup(s => s.GetCharacterByIdAsync(characterId))
            .ReturnsAsync(character);

        // Act
        var result = await _controller.GetCharacterById(characterId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedCharacter = Assert.IsType<CharacterEntity>(okResult.Value);
        Assert.Equal(characterId, returnedCharacter.Id);
        Assert.Equal("Rick Sanchez", returnedCharacter.Name);
        Assert.Equal("Alive", returnedCharacter.Status);
    }

    [Fact]
    public async Task UpdateCharacter_WithValidRequest_ShouldReturnOkWithUpdatedCharacter()
    {
        // Arrange
        var characterId = 1;
        var updateRequest = new CharacterUpdateRequest
        {
            Status = "Dead",
            EpisodeCount = 41
        };

        var updatedCharacter = new CharacterEntity
        {
            Id = characterId,
            Name = "Rick Sanchez",
            Status = "Dead",
            EpisodeCount = 41
        };

        var validationResult = new ValidationResult(true);
        _mockValidationService.Setup(v => v.ValidateUpdateRequest(updateRequest))
            .Returns(validationResult);

        _mockCharacterService.Setup(s => s.UpdateCharacterAsync(characterId, updateRequest))
            .ReturnsAsync(updatedCharacter);

        // Act
        var result = await _controller.UpdateCharacter(characterId, updateRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedCharacter = Assert.IsType<CharacterEntity>(okResult.Value);
        Assert.Equal("Dead", returnedCharacter.Status);
        Assert.Equal(41, returnedCharacter.EpisodeCount);
    }
}
