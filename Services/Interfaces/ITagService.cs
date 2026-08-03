using ToDoApp.Api.DTOs.Tags;

namespace ToDoApp.Api.Services.Interfaces;

public interface ITagService
{
    Task<IReadOnlyList<TagDto>> GetAllAsync();
    Task<TagDto> CreateAsync(CreateTagDto dto);
}
