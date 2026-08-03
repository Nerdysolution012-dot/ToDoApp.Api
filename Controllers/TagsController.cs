using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Api.DTOs.Tags;
using ToDoApp.Api.Services.Interfaces;

namespace ToDoApp.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/tags")]
public class TagsController(ITagService tagService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TagDto>>> GetAll()
        => Ok(await tagService.GetAllAsync());

    [HttpPost]
    public async Task<ActionResult<TagDto>> Create(CreateTagDto dto)
    {
        var created = await tagService.CreateAsync(dto);
        return StatusCode(StatusCodes.Status201Created, created);
    }
}
