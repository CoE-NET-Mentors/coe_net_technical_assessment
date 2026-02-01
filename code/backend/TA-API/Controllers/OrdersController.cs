using Microsoft.AspNetCore.Mvc;
using TA_API.Models.Requests;

namespace TA_API.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(new { message = "Orders endpoint placeholder" });
    }

    [HttpGet("{id}")]
    public IActionResult GetById(string id)
    {
        return Ok(new { message = "Orders endpoint placeholder", id });
    }

    [HttpPost]
    public IActionResult Create([FromBody] OrderCreateRequest request)
    {
        // TODO: Validate request fields as part of the assessment.
        return Ok(new { message = "Orders endpoint placeholder" });
    }

    [HttpPut("{id}")]
    public IActionResult Update(string id, [FromBody] OrderUpdateRequest request)
    {
        // TODO: Validate request fields as part of the assessment.
        return Ok(new { message = "Orders endpoint placeholder", id });
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        return Ok(new { message = "Orders endpoint placeholder", id });
    }
}
