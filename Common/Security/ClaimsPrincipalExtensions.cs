using System.Security.Claims;
using ToDoApp.Api.Common.Exceptions;
using ToDoApp.Api.DTOs.Common;

namespace ToDoApp.Api.Common.Security;

public static class ClaimsPrincipalExtensions
{
    public static CallerContext ToCallerContext(this ClaimsPrincipal principal)
    {
        var idValue = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = principal.FindFirstValue(ClaimTypes.Email);
        var role = principal.FindFirstValue(ClaimTypes.Role);

        if (!int.TryParse(idValue, out var userId) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(role))
        {
            throw new UnauthorizedException("The access token does not contain the required claims.");
        }

        return new CallerContext(userId, email, role);
    }
}
