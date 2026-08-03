using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Api.Models;

public class TaskItem
{
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Notes { get; set; }

    [Required]
    public DateTime DueDate { get; set; }

    [Range(1, 3)]
    public int Priority { get; set; }

    public bool IsCompleted { get; set; } = false;

    [Required]
    public int TaskListId { get; set; }

    public TaskList TaskList { get; set; } = null!;
    public ICollection<TaskTag> TaskTags { get; set; } = new List<TaskTag>();
}
