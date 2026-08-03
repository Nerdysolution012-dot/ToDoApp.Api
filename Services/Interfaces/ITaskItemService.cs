using ToDoApp.Api.DTOs.Common;
using ToDoApp.Api.DTOs.TaskItems;

namespace ToDoApp.Api.Services.Interfaces;

public interface ITaskItemService
{
    Task<IReadOnlyList<TaskItemDto>> GetByListAsync(int listId, CallerContext caller);
    Task<TaskItemDto> GetByIdAsync(int id, CallerContext caller);
    Task<TaskItemDto> CreateAsync(CreateTaskItemDto dto, CallerContext caller);
    Task UpdateAsync(int id, UpdateTaskItemDto dto, CallerContext caller);
    Task DeleteAsync(int id, CallerContext caller);
}
