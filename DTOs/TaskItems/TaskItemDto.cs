namespace ToDoApp.Api.DTOs.TaskItems;

public class TaskItemDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime DueDate { get; set; }
    public int Priority { get; set; }
    public bool IsCompleted { get; set; }
    public int TaskListId { get; set; }
    public string TaskListName { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = [];
}
