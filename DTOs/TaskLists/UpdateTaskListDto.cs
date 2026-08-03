using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Api.DTOs.TaskLists;

public class UpdateTaskListDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(300)]
    public string? Description { get; set; }
}
