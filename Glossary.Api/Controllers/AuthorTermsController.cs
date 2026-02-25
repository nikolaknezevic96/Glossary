using Glossary.Api.Security;
using Glossary.Application.Dtos;
using Glossary.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Glossary.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/author/terms")]
public sealed class AuthorTermsController : ControllerBase
{
    private readonly IGlossaryService _service;

    public AuthorTermsController(IGlossaryService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var items = await _service.GetAllAsync(ct);
        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTermRequest request, CancellationToken ct)
    {
        var userId = User.GetUserId();
        var created = await _service.CreateAsync(userId, request, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var item = await _service.GetByIdAsync(id, ct);
        return Ok(item);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateDraft(Guid id, UpdateTermRequest request, CancellationToken ct)
    {
        var userId = User.GetUserId();
        var updated = await _service.UpdateDraftAsync(userId, id, request, ct);
        return Ok(updated);
    }

    [HttpPost("{id:guid}/publish")]
    public async Task<IActionResult> Publish(Guid id, CancellationToken ct)
    {
        var userId = User.GetUserId();
        await _service.PublishAsync(userId, id, DateTimeOffset.UtcNow, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var userId = User.GetUserId();
        await _service.DeleteDraftAsync(userId, id, ct);
        return NoContent();
    }
}
