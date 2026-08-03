using Microsoft.EntityFrameworkCore;
using ToDoApp.Api.Common.Exceptions;

namespace ToDoApp.Api.Common.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (AppException ex)
        {
            await WriteErrorAsync(context, ex.StatusCode, ex.Message);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "A database update failed.");
            await WriteErrorAsync(context, StatusCodes.Status409Conflict,
                "The request conflicts with existing database data.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled error occurred.");
            await WriteErrorAsync(context, StatusCodes.Status500InternalServerError,
                "An unexpected server error occurred.");
        }
    }

    private static async Task WriteErrorAsync(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new
        {
            statusCode,
            message,
            traceId = context.TraceIdentifier
        });
    }
}
