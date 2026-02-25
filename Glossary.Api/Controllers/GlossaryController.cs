using Glossary.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Glossary.Api.Controllers;

[ApiController]
[Route("api/glossary")]
public sealed class GlossaryController : ControllerBase
{
    private readonly IGlossaryService _service;

    public GlossaryController(IGlossaryService service)
    {
               _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetPublished(CancellationToken ct)
    {
        var items = await _service.GetPublishedAsync(ct);
        return Ok(items);
    }
}
