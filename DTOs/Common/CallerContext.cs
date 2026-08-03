namespace ToDoApp.Api.DTOs.Common;

public sealed record CallerContext(int UserId, string Email, string Role)
{
    public bool IsAdmin => string.Equals(Role, "Admin", StringComparison.OrdinalIgnoreCase);
}
