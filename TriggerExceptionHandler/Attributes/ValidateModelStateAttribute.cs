using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace TriggerExceptionHandler.Attributes
{
    public sealed class ValidateModelStateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var result = context.HttpContext.RequestServices.GetRequiredService<ValidationProblemDetailsResult>();
                context.Result = result;
            }
        }
    }
}