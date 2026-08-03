namespace ToDoApp.Api.Common.Exceptions;

public sealed class ForbiddenException(string message) : AppException(message)
{
    public override int StatusCode => StatusCodes.Status403Forbidden;
}
