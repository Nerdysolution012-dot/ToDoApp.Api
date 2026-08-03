using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Api.DTOs.TaskLists;

public class CreateTaskListDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(300)]
    public string? Description { get; set; }

    // Members cannot choose this value. Admins may use it to create a list for another user.
    public int? UserId { get; set; }
}
