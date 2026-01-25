using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Commerce.Api.Validation;

public sealed class FluentValidationActionFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var arg in context.ActionArguments.Values)
        {
            if (arg is null) continue;

            // Look up IValidator<TArg> for this argument type
            var validatorType = typeof(IValidator<>).MakeGenericType(arg.GetType());
            var validatorObj = context.HttpContext.RequestServices.GetService(validatorType);

            if (validatorObj is not IValidator validator)
                continue; // no validator registered for this arg, skip

            var validationContext = new ValidationContext<object>(arg);

            var result = await validator.ValidateAsync(validationContext, context.HttpContext.RequestAborted);

            if (result.IsValid)
                continue;

            var errors = result.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            context.Result = new BadRequestObjectResult(new ValidationProblemDetails(errors));
            return;
        }

        await next();
    }
}
