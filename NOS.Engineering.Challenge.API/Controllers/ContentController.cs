using System.Net;
using Microsoft.AspNetCore.Mvc;
using NOS.Engineering.Challenge.API.Models;
using NOS.Engineering.Challenge.Managers;

namespace NOS.Engineering.Challenge.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class ContentController : Controller
{
    private readonly IContentsManager _manager;
    public ContentController(IContentsManager manager)
    {
        _manager = manager;
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchContents([FromQuery] string? title, [FromQuery] string? genre)
    {
        var filteredContents = await _manager.SearchContents(title, genre).ConfigureAwait(false);

        if (!filteredContents.Any())
            return NotFound();

        return Ok(filteredContents);
    }

    [Obsolete("This endpoint is deprecated. Use GET /api/v1/Content/search instead.")]
    [HttpGet]
    public async Task<IActionResult> GetManyContents()
    {
        var contents = await _manager.GetManyContents().ConfigureAwait(false);

        if (!contents.Any())
            return NotFound();
        
        return Ok(contents);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetContent(Guid id)
    {
        var content = await _manager.GetContent(id).ConfigureAwait(false);

        if (content == null)
            return NotFound();
        
        return Ok(content);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateContent(
        [FromBody] ContentInput content
        )
    {
        var createdContent = await _manager.CreateContent(content.ToDto()).ConfigureAwait(false);

        return createdContent == null ? Problem() : Ok(createdContent);
    }
    
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateContent(
        Guid id,
        [FromBody] ContentInput content
        )
    {
        var updatedContent = await _manager.UpdateContent(id, content.ToDto()).ConfigureAwait(false);

        return updatedContent == null ? NotFound() : Ok(updatedContent);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteContent(
        Guid id
    )
    {
        var deletedId = await _manager.DeleteContent(id).ConfigureAwait(false);
        return Ok(deletedId);
    }

    [HttpPost("{id}/genre")]
    public async Task<IActionResult> AddGenres(Guid id, [FromBody] IEnumerable<string> genres)
    {
        var updatedContent = await _manager.AddGenresToContent(id, genres).ConfigureAwait(false);

        return updatedContent == null ? NotFound() : Ok(updatedContent);
    }

    [HttpDelete("{id}/genre")]
    public async Task<IActionResult> RemoveGenres(Guid id, [FromBody] IEnumerable<string> genres)
    {
        var updatedContent = await _manager.RemoveGenresFromContent(id, genres).ConfigureAwait(false);

        return updatedContent == null ? NotFound() : Ok(updatedContent);
    }
}