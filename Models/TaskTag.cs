namespace ToDoApp.Api.Models;

public class TaskTag
{
    public int Id { get; set; }
    public int TaskItemId { get; set; }
    public int TagId { get; set; }
    public DateTime TaggedAt { get; set; } = DateTime.Now;

    public TaskItem TaskItem { get; set; } = null!;
    public Tag Tag { get; set; } = null!;
}
