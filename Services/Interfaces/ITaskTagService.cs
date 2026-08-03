using ToDoApp.Api.DTOs.Common;
using ToDoApp.Api.DTOs.Tags;

namespace ToDoApp.Api.Services.Interfaces;

public interface ITaskTagService
{
    Task<TaskTagDto> AssignAsync(int taskId, AssignTagDto dto, CallerContext caller);
    Task<IReadOnlyList<TaskTagDto>> GetByTaskAsync(int taskId, CallerContext caller);
}
