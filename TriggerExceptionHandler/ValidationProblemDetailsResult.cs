using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;
using TriggerExceptionHandler.Extensions;

namespace TriggerExceptionHandler
{
    public class ValidationProblemDetailsResult : IActionResult
    {
        private readonly ILogger _logger;

        public ValidationProblemDetailsResult()
        {
        }

        public ValidationProblemDetailsResult(ILogger<ValidationProblemDetailsResult> logger)
        {
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
                //Instance = $"urn:{_applicationName}:{eventId.Id.ToString()}",
                Status = (int)HttpStatusCode.BadRequest,
                Type = nameof(ValidationProblemDetails),
                Title = "Request Validation Error",
            };

            _logger?.LogWarning(eventId: eventId, message: problemDetails.Detail, problemDetails, context);

            context.HttpContext.Response.StatusCode = problemDetails.Status.Value;
            await context.HttpContext.Response.WriteJsonAsync(problemDetails).ConfigureAwait(false);
        }
    }
}