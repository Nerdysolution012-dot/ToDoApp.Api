namespace ToDoApp.Api.DTOs.TaskLists;

public class TaskListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public int UserId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public int TaskCount { get; set; }
}
