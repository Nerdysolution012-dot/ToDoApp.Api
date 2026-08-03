using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Api.DTOs.Tags;

public class AssignTagDto
{
    [Required]
    public int TagId { get; set; }
}
