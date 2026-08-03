using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Api.Models;

public class TaskList
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(300)]
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Required]
    public int UserId { get; set; }

    public User User { get; set; } = null!;
    public ICollection<TaskItem> TaskItems { get; set; } = new List<TaskItem>();
}
