namespace ToDoApp.Api.Common.Exceptions;

public sealed class BadRequestException(string message) : AppException(message)
{
    public override int StatusCode => StatusCodes.Status400BadRequest;
}
