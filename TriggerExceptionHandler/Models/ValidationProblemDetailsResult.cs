using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TriggerExceptionHandler.Extensions;

namespace TriggerExceptionHandler.Models
{

    public class ValidationProblemDetailsResult : IActionResult
    {
        private readonly string _applicationName;
        private static ILogger _logger;

        public ValidationProblemDetailsResult(string applicationName = null, ILogger logger = null)
        {
            _applicationName = applicationName;
            _logger = logger;
        }

        /// <summary>
        /// Invoked from <see cref="TriggerExceptionHandler.Attributes.ValidateModelStateAttribute"/>
        /// </summary>
        public async Task ExecuteResultAsync(ActionContext context)
        {
            var eventId = new EventId(context.HttpContext.TraceIdentifier.GetHashCode(), nameof(ValidationProblemDetails));

            var problemDetails = new ValidationProblemDetails(context.ModelState)
            {
                Detail = "One or more validation errors occurred",
                Instance = $"urn:{_applicationName}:{eventId.Id.ToString()}",
                Status = 400,
                Type = typeof(ValidationProblemDetails).Name,
                Title = "Request Validation Error",
            };

            _logger?.LogWarning( eventId: eventId, message: problemDetails.Detail,problemDetails, context);

            context.HttpContext.Response.StatusCode = problemDetails.Status.Value;
            await context.HttpContext.Response.WriteJsonAsync(problemDetails);
        }
    }
}