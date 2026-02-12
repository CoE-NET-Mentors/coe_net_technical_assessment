using Microsoft.AspNetCore.Mvc;
using TA_API.Services.Characters;
using TA_API.Models.Data;

namespace TA_API.Controllers;

[ApiController]
[Route("/public/characters")]
public class PublicCharactersController : ControllerBase
{
    private readonly ICharacterService _characterService;

    public PublicCharactersController(ICharacterService characterService)
    {
        _characterService = characterService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _characterService.GetAllCharactersAsync();
        return Ok(new { count = list.Count, data = list });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var character = await _characterService.GetCharacterByIdAsync(id);
        if (character == null) return NotFound();
        return Ok(character);
    }
}
