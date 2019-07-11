using TriggerExceptionHandler.Models;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TriggerExceptionHandler.Attributes
{
    public class ValidateModelStateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
                context.Result = new ValidationProblemDetailsResult();
        }
    }
}
