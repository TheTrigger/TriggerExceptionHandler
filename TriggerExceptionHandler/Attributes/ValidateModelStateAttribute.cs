using Microsoft.AspNetCore.Mvc.Filters;

namespace TriggerExceptionHandler.Attributes
{
    public sealed class ValidateModelStateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
                context.Result = new ValidationProblemDetailsResult();
        }
    }
}