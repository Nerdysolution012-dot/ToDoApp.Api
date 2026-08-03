using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Api.Models;

public class Tag
{
    public int Id { get; set; }

    [Required]
    [StringLength(30)]
    public string Name { get; set; } = string.Empty;

    public ICollection<TaskTag> TaskTags { get; set; } = new List<TaskTag>();
}
