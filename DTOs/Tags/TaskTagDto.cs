namespace ToDoApp.Api.DTOs.Tags;

public class TaskTagDto
{
    public int AssignmentId { get; set; }
    public int TaskItemId { get; set; }
    public int TagId { get; set; }
    public string TagName { get; set; } = string.Empty;
    public DateTime TaggedAt { get; set; }
}
