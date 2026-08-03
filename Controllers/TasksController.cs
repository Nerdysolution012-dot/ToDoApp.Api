using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Api.Common.Security;
using ToDoApp.Api.DTOs.TaskItems;
using ToDoApp.Api.DTOs.Tags;
using ToDoApp.Api.Services.Interfaces;

namespace ToDoApp.Api.Controllers;

[ApiController]
[Authorize]
public class TasksController(
    ITaskItemService taskItemService,
    ITaskTagService taskTagService) : ControllerBase
{
    [HttpGet("api/tasklists/{listId:int}/tasks")]
    public async Task<ActionResult<IReadOnlyList<TaskItemDto>>> GetByList(int listId)
        => Ok(await taskItemService.GetByListAsync(listId, User.ToCallerContext()));

    [HttpGet("api/tasks/{id:int}")]
    public async Task<ActionResult<TaskItemDto>> GetById(int id)
        => Ok(await taskItemService.GetByIdAsync(id, User.ToCallerContext()));

    [HttpPost("api/tasks")]
    public async Task<ActionResult<TaskItemDto>> Create(CreateTaskItemDto dto)
    {
        var created = await taskItemService.CreateAsync(dto, User.ToCallerContext());
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("api/tasks/{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateTaskItemDto dto)
    {
        await taskItemService.UpdateAsync(id, dto, User.ToCallerContext());
        return NoContent();
    }

    [HttpDelete("api/tasks/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await taskItemService.DeleteAsync(id, User.ToCallerContext());
        return NoContent();
    }

    [HttpPost("api/tasks/{taskId:int}/tags")]
    public async Task<ActionResult<TaskTagDto>> AssignTag(int taskId, AssignTagDto dto)
    {
        var created = await taskTagService.AssignAsync(taskId, dto, User.ToCallerContext());
        return StatusCode(StatusCodes.Status201Created, created);
    }

    [HttpGet("api/tasks/{taskId:int}/tags")]
    public async Task<ActionResult<IReadOnlyList<TaskTagDto>>> GetTags(int taskId)
        => Ok(await taskTagService.GetByTaskAsync(taskId, User.ToCallerContext()));
}
