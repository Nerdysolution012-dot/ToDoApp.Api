using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Api.DTOs.TaskItems;

public class CreateTaskItemDto
{
    [Required]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Notes { get; set; }

    [Required]
    public DateTime DueDate { get; set; }

    [Range(1, 3)]
    public int Priority { get; set; }

    [Required]
    public int TaskListId { get; set; }
}
