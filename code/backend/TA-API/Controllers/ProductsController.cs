using Microsoft.AspNetCore.Mvc;
using TA_API.Models.Requests;

namespace TA_API.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(new { message = "Products endpoint placeholder" });
    }

    [HttpGet("{id}")]
    public IActionResult GetById(string id)
    {
        return Ok(new { message = "Products endpoint placeholder", id });
    }

    [HttpPost]
    public IActionResult Create([FromBody] ProductCreateRequest request)
    {
        // TODO: Validate request fields as part of the assessment.
        return Ok(new { message = "Products endpoint placeholder" });
    }

    [HttpPut("{id}")]
    public IActionResult Update(string id, [FromBody] ProductUpdateRequest request)
    {
        // TODO: Validate request fields as part of the assessment.
        return Ok(new { message = "Products endpoint placeholder", id });
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        return Ok(new { message = "Products endpoint placeholder", id });
    }
}
