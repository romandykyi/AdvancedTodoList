using AdvancedTodoList.Application.Services.Definitions;
using Microsoft.AspNetCore.Mvc;

namespace AdvancedTodoList.WebApp.Extensions;

public static class ServiceResponseExtensions
{
    public static IActionResult ToActionResult<T>(this ServiceResponse<T> response)
    {
        return response.Status switch
        {
            ServiceResponseStatus.Success =>
                response.Result != null ? new OkObjectResult(response.Result) : new NoContentResult(),
            ServiceResponseStatus.NotFound => new NotFoundResult(),
            ServiceResponseStatus.Forbidden => new ForbidResult(),
            _ => throw new ArgumentException("Invalid service response status", nameof(response))
        };
    }
    public static IActionResult ToActionResult(this ServiceResponseStatus status)
    {
        return status switch
        {
            ServiceResponseStatus.Success => new NoContentResult(),
            ServiceResponseStatus.NotFound => new NotFoundResult(),
            ServiceResponseStatus.Forbidden => new ForbidResult(),
            _ => throw new ArgumentException("Invalid service response status", nameof(status))
        };
    }
}
