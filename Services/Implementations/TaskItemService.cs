using Microsoft.EntityFrameworkCore;
using ToDoApp.Api.Common.Exceptions;
using ToDoApp.Api.Data;
using ToDoApp.Api.DTOs.Common;
using ToDoApp.Api.DTOs.TaskItems;
using ToDoApp.Api.Models;
using ToDoApp.Api.Services.Interfaces;

namespace ToDoApp.Api.Services.Implementations;

public class TaskItemService(AppDbContext db) : ITaskItemService
{
    public async Task<IReadOnlyList<TaskItemDto>> GetByListAsync(int listId, CallerContext caller)
    {
        var list = await db.TaskLists.AsNoTracking().SingleOrDefaultAsync(x => x.Id == listId);
        if (list is null)
        {
            throw new NotFoundException("Task list was not found");
        }

        EnsureOwnership(list.UserId, caller);

        return await ProjectTaskItems()
            .Where(x => x.TaskListId == listId)
            .OrderBy(x => x.IsCompleted)
            .ThenBy(x => x.DueDate)
            .ToListAsync();
    }

    public async Task<TaskItemDto> GetByIdAsync(int id, CallerContext caller)
    {
        var ownerData = await db.TaskItems.AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new { x.Id, OwnerId = x.TaskList.UserId })
            .SingleOrDefaultAsync();

        if (ownerData is null)
        {
            throw new NotFoundException("Task item was not found");
        }

        EnsureOwnership(ownerData.OwnerId, caller);

        return await ProjectTaskItems().SingleAsync(x => x.Id == id);
    }

    public async Task<TaskItemDto> CreateAsync(CreateTaskItemDto dto, CallerContext caller)
    {
        ValidateDueDate(dto.DueDate);

        var list = await db.TaskLists.AsNoTracking().SingleOrDefaultAsync(x => x.Id == dto.TaskListId);
        if (list is null)
        {
            throw new BadRequestException("Task list does not exist");
        }

        EnsureOwnership(list.UserId, caller);

        var entity = new TaskItem
        {
            Title = dto.Title.Trim(),
            Notes = dto.Notes?.Trim(),
            DueDate = dto.DueDate,
            Priority = dto.Priority,
            IsCompleted = false,
            TaskListId = dto.TaskListId
        };

        db.TaskItems.Add(entity);
        await db.SaveChangesAsync();

        return await GetByIdAsync(entity.Id, caller);
    }

    public async Task UpdateAsync(int id, UpdateTaskItemDto dto, CallerContext caller)
    {
        ValidateDueDate(dto.DueDate);

        var entity = await db.TaskItems
            .Include(x => x.TaskList)
            .SingleOrDefaultAsync(x => x.Id == id);

        if (entity is null)
        {
            throw new NotFoundException("Task item was not found");
        }

        EnsureOwnership(entity.TaskList.UserId, caller);

        var targetList = await db.TaskLists.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == dto.TaskListId);

        if (targetList is null)
        {
            throw new BadRequestException("Task list does not exist");
        }

        // A member must also own the destination list when moving a task.
        EnsureOwnership(targetList.UserId, caller);

        entity.Title = dto.Title.Trim();
        entity.Notes = dto.Notes?.Trim();
        entity.DueDate = dto.DueDate;
        entity.Priority = dto.Priority;
        entity.IsCompleted = dto.IsCompleted;
        entity.TaskListId = dto.TaskListId;

        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id, CallerContext caller)
    {
        var entity = await db.TaskItems
            .Include(x => x.TaskList)
            .SingleOrDefaultAsync(x => x.Id == id);

        if (entity is null)
        {
            throw new NotFoundException("Task item was not found");
        }

        EnsureOwnership(entity.TaskList.UserId, caller);
        db.TaskItems.Remove(entity);
        await db.SaveChangesAsync();
    }

    private IQueryable<TaskItemDto> ProjectTaskItems() =>
        db.TaskItems.AsNoTracking().Select(x => new TaskItemDto
        {
            Id = x.Id,
            Title = x.Title,
            Notes = x.Notes,
            DueDate = x.DueDate,
            Priority = x.Priority,
            IsCompleted = x.IsCompleted,
            TaskListId = x.TaskListId,
            TaskListName = x.TaskList.Name,
            Tags = x.TaskTags
                .OrderBy(tt => tt.Tag.Name)
                .Select(tt => tt.Tag.Name)
                .ToList()
        });

    private static void ValidateDueDate(DateTime dueDate)
    {
        if (dueDate < DateTime.Now)
        {
            throw new BadRequestException("Due date cannot be in the past");
        }
    }

    private static void EnsureOwnership(int ownerId, CallerContext caller)
    {
        if (!caller.IsAdmin && ownerId != caller.UserId)
        {
            throw new ForbiddenException("You do not own this task item");
        }
    }
}
