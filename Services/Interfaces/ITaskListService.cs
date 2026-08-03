using ToDoApp.Api.DTOs.Common;
using ToDoApp.Api.DTOs.TaskLists;

namespace ToDoApp.Api.Services.Interfaces;

public interface ITaskListService
{
    Task<IReadOnlyList<TaskListDto>> GetAllAsync(CallerContext caller);
    Task<TaskListDto> GetByIdAsync(int id, CallerContext caller);
    Task<TaskListDto> CreateAsync(CreateTaskListDto dto, CallerContext caller);
    Task UpdateAsync(int id, UpdateTaskListDto dto, CallerContext caller);
    Task DeleteAsync(int id, CallerContext caller);
}
