using Commerce.Shared.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Commerce.Api.Mappers;

public static class ControllerDbResultExtensions
{
    public static IActionResult ToActionResult(
        this ControllerBase controller,
        DbResultOption result,
        Func<IActionResult> onSuccess)
    {
        return result switch
        {
            DbResultOption.Success => onSuccess(),
            DbResultOption.NotFound => controller.NotFound(),
            DbResultOption.AlreadyExists => controller.Conflict("Resource already exists."),
            DbResultOption.Invalid => controller.BadRequest("Invalid data provided."),
            DbResultOption.Conflict => controller.Conflict("Conflict occurred while processing the request."),
            DbResultOption.Error => controller.StatusCode(500, "An internal server error occurred."),
            _ => controller.StatusCode(500, "An unknown error occurred.")
        };
    }
}
