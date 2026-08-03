using Microsoft.EntityFrameworkCore;
using ToDoApp.Api.Common.Exceptions;
using ToDoApp.Api.Data;
using ToDoApp.Api.DTOs.Common;
using ToDoApp.Api.DTOs.TaskLists;
using ToDoApp.Api.Models;
using ToDoApp.Api.Services.Interfaces;

namespace ToDoApp.Api.Services.Implementations;

public class TaskListService(AppDbContext db) : ITaskListService
{
    public async Task<IReadOnlyList<TaskListDto>> GetAllAsync(CallerContext caller)
    {
        var query = db.TaskLists.AsNoTracking();

        if (!caller.IsAdmin)
        {
            query = query.Where(x => x.UserId == caller.UserId);
        }

        return await query
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new TaskListDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                CreatedAt = x.CreatedAt,
                UserId = x.UserId,
                OwnerName = x.User.FullName,
                TaskCount = x.TaskItems.Count
            })
            .ToListAsync();
    }

    public async Task<TaskListDto> GetByIdAsync(int id, CallerContext caller)
    {
        var list = await db.TaskLists.AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new TaskListDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                CreatedAt = x.CreatedAt,
                UserId = x.UserId,
                OwnerName = x.User.FullName,
                TaskCount = x.TaskItems.Count
            })
            .SingleOrDefaultAsync();

        if (list is null)
        {
            throw new NotFoundException("Task list was not found");
        }

        EnsureOwnership(list.UserId, caller);
        return list;
    }

    public async Task<TaskListDto> CreateAsync(CreateTaskListDto dto, CallerContext caller)
    {
        // Members always create for themselves. Only an Admin can supply another UserId.
        var ownerId = caller.IsAdmin && dto.UserId.HasValue ? dto.UserId.Value : caller.UserId;

        if (!await db.Users.AnyAsync(x => x.Id == ownerId))
        {
            throw new BadRequestException("User does not exist");
        }

        var entity = new TaskList
        {
            Name = dto.Name.Trim(),
            Description = dto.Description?.Trim(),
            UserId = ownerId,
            CreatedAt = DateTime.Now
        };

        db.TaskLists.Add(entity);
        await db.SaveChangesAsync();

        return await GetByIdAsync(entity.Id, caller.IsAdmin
            ? caller
            : new CallerContext(ownerId, caller.Email, caller.Role));
    }

    public async Task UpdateAsync(int id, UpdateTaskListDto dto, CallerContext caller)
    {
        var entity = await db.TaskLists.SingleOrDefaultAsync(x => x.Id == id);
        if (entity is null)
        {
            throw new NotFoundException("Task list was not found");
        }

        EnsureOwnership(entity.UserId, caller);

        entity.Name = dto.Name.Trim();
        entity.Description = dto.Description?.Trim();
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id, CallerContext caller)
    {
        var entity = await db.TaskLists.SingleOrDefaultAsync(x => x.Id == id);
        if (entity is null)
        {
            throw new NotFoundException("Task list was not found");
        }

        EnsureOwnership(entity.UserId, caller);

        db.TaskLists.Remove(entity);
        await db.SaveChangesAsync();
    }

    private static void EnsureOwnership(int ownerId, CallerContext caller)
    {
        // [Authorize] proves the caller is signed in; this database-backed check proves ownership.
        if (!caller.IsAdmin && ownerId != caller.UserId)
        {
            throw new ForbiddenException("You do not own this task list");
        }
    }
}
