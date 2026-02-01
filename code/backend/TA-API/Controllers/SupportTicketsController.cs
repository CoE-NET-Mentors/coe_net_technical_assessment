using Microsoft.AspNetCore.Mvc;
using TA_API.Models.Requests;

namespace TA_API.Controllers;

[ApiController]
[Route("api/support-tickets")]
public class SupportTicketsController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(new { message = "Support tickets endpoint placeholder" });
    }

    [HttpGet("{id}")]
    public IActionResult GetById(string id)
    {
        return Ok(new { message = "Support tickets endpoint placeholder", id });
    }

    [HttpPost]
    public IActionResult Create([FromBody] SupportTicketCreateRequest request)
    {
        // TODO: Validate request fields as part of the assessment.
        return Ok(new { message = "Support tickets endpoint placeholder" });
    }

    [HttpPut("{id}")]
    public IActionResult Update(string id, [FromBody] SupportTicketUpdateRequest request)
    {
        // TODO: Validate request fields as part of the assessment.
        return Ok(new { message = "Support tickets endpoint placeholder", id });
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        return Ok(new { message = "Support tickets endpoint placeholder", id });
    }
}
