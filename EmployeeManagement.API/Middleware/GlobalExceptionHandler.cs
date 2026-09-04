using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.API.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        // Set up the blueprint for our clean JSON error
        var problemDetails = new ProblemDetails
        {
            Instance = httpContext.Request.Path
        };

        // If it is our Validator throwing the error, format it as a 400 Bad Request
        if (exception is ArgumentException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            problemDetails.Title = "Validation Error";
            problemDetails.Detail = exception.Message;
            problemDetails.Status = StatusCodes.Status400BadRequest;
        }
        // If it is any other random server crash, format it as a 500 Internal Server Error
        else if (exception is KeyNotFoundException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            problemDetails.Title = "Resource Not Found";
            problemDetails.Detail = exception.Message;
            problemDetails.Status = StatusCodes.Status404NotFound;
        }
        else
        {
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            problemDetails.Title = "Internal Server Error";
            problemDetails.Detail = "An unexpected error occurred.";
            problemDetails.Status = StatusCodes.Status500InternalServerError;
        }

        // Send the beautiful JSON back to the user
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true; // Tells .NET "I handled this error, don't crash!"
    }
}
