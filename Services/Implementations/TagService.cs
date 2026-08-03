using Microsoft.EntityFrameworkCore;
using ToDoApp.Api.Common.Exceptions;
using ToDoApp.Api.Data;
using ToDoApp.Api.DTOs.Tags;
using ToDoApp.Api.Models;
using ToDoApp.Api.Services.Interfaces;

namespace ToDoApp.Api.Services.Implementations;

public class TagService(AppDbContext db) : ITagService
{
    public async Task<IReadOnlyList<TagDto>> GetAllAsync() =>
        await db.Tags.AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new TagDto { Id = x.Id, Name = x.Name })
            .ToListAsync();

    public async Task<TagDto> CreateAsync(CreateTagDto dto)
    {
        var name = dto.Name.Trim();

        if (await db.Tags.AnyAsync(x => x.Name == name))
        {
            throw new ConflictException("Tag name already exists");
        }

        var entity = new Tag { Name = name };
        db.Tags.Add(entity);

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new ConflictException("Tag name already exists");
        }

        return new TagDto { Id = entity.Id, Name = entity.Name };
    }
}
