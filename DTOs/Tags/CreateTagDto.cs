using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Api.DTOs.Tags;

public class CreateTagDto
{
    [Required]
    [StringLength(30)]
    public string Name { get; set; } = string.Empty;
}
