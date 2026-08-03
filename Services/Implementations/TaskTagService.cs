using Microsoft.EntityFrameworkCore;
using ToDoApp.Api.Common.Exceptions;
using ToDoApp.Api.Data;
using ToDoApp.Api.DTOs.Common;
using ToDoApp.Api.DTOs.Tags;
using ToDoApp.Api.Models;
using ToDoApp.Api.Services.Interfaces;

namespace ToDoApp.Api.Services.Implementations;

public class TaskTagService(AppDbContext db) : ITaskTagService
{
    public async Task<TaskTagDto> AssignAsync(int taskId, AssignTagDto dto, CallerContext caller)
    {
        var task = await db.TaskItems.AsNoTracking()
            .Where(x => x.Id == taskId)
            .Select(x => new { x.Id, OwnerId = x.TaskList.UserId })
            .SingleOrDefaultAsync();

        if (task is null)
        {
            throw new NotFoundException("Task item was not found");
        }

        EnsureOwnership(task.OwnerId, caller);

        var tag = await db.Tags.AsNoTracking().SingleOrDefaultAsync(x => x.Id == dto.TagId);
        if (tag is null)
        {
            throw new NotFoundException("Tag was not found");
        }

        if (await db.TaskTags.AnyAsync(x => x.TaskItemId == taskId && x.TagId == dto.TagId))
        {
            throw new ConflictException("This tag is already assigned to the task");
        }

        var entity = new TaskTag
        {
            TaskItemId = taskId,
            TagId = dto.TagId,
            TaggedAt = DateTime.Now
        };

        db.TaskTags.Add(entity);

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new ConflictException("This tag is already assigned to the task");
        }

        return new TaskTagDto
        {
            AssignmentId = entity.Id,
            TaskItemId = entity.TaskItemId,
            TagId = entity.TagId,
            TagName = tag.Name,
            TaggedAt = entity.TaggedAt
        };
    }

    public async Task<IReadOnlyList<TaskTagDto>> GetByTaskAsync(int taskId, CallerContext caller)
    {
        var task = await db.TaskItems.AsNoTracking()
            .Where(x => x.Id == taskId)
            .Select(x => new { x.Id, OwnerId = x.TaskList.UserId })
            .SingleOrDefaultAsync();

        if (task is null)
        {
            throw new NotFoundException("Task item was not found");
        }

        EnsureOwnership(task.OwnerId, caller);

        return await db.TaskTags.AsNoTracking()
            .Where(x => x.TaskItemId == taskId)
            .OrderBy(x => x.Tag.Name)
            .Select(x => new TaskTagDto
            {
                AssignmentId = x.Id,
                TaskItemId = x.TaskItemId,
                TagId = x.TagId,
                TagName = x.Tag.Name,
                TaggedAt = x.TaggedAt
            })
            .ToListAsync();
    }

    private static void EnsureOwnership(int ownerId, CallerContext caller)
    {
        if (!caller.IsAdmin && ownerId != caller.UserId)
        {
            throw new ForbiddenException("You do not own this task item");
        }
    }
}
