using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Api.Common.Security;
using ToDoApp.Api.DTOs.TaskLists;
using ToDoApp.Api.Services.Interfaces;

namespace ToDoApp.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/tasklists")]
public class TaskListsController(ITaskListService taskListService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TaskListDto>>> GetAll()
        => Ok(await taskListService.GetAllAsync(User.ToCallerContext()));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskListDto>> GetById(int id)
        => Ok(await taskListService.GetByIdAsync(id, User.ToCallerContext()));

    [HttpPost]
    public async Task<ActionResult<TaskListDto>> Create(CreateTaskListDto dto)
    {
        var created = await taskListService.CreateAsync(dto, User.ToCallerContext());
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateTaskListDto dto)
    {
        await taskListService.UpdateAsync(id, dto, User.ToCallerContext());
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await taskListService.DeleteAsync(id, User.ToCallerContext());
        return NoContent();
    }
}
