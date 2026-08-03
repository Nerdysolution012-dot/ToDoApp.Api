using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Api.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^(Member|Admin)$", ErrorMessage = "Role must be Member or Admin.")]
    public string Role { get; set; } = RoleNames.Member;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<TaskList> TaskLists { get; set; } = new List<TaskList>();
}
