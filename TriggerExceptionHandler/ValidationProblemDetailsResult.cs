using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace TriggerExceptionHandler
{
    public class ValidationProblemDetailsResult : IActionResult
    {
        private readonly ILogger _logger;
        private readonly IOptions<JsonOptions> _jsonOptions;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public ValidationProblemDetailsResult(ILogger<ValidationProblemDetailsResult> logger, IOptions<JsonOptions> jsonOptions)
        {
            _logger = logger;
            _jsonOptions = jsonOptions;
            _jsonSerializerOptions = _jsonOptions.Value.JsonSerializerOptions;
        }

        /// <summary>
        /// Invoked from <see cref="TriggerExceptionService.Attributes.ValidateModelStateAttribute"/>
        /// </summary>
        public Task ExecuteResultAsync(ActionContext context)
        {
            var httpContext = context.HttpContext;
            
            var problemDetails = new ValidationProblemDetails(context.ModelState)
            {
                Detail = "One or more validation errors occurred",
                Status = (int)HttpStatusCode.BadRequest,
                Type = nameof(ValidationProblemDetails),
                Title = "Request Validation Error",
            };

            _logger.LogWarning(message: "Validation Problem: {title} {details}", problemDetails.Title, problemDetails.Detail);

            httpContext.Response.StatusCode = problemDetails.Status.Value;
            return httpContext.Response.WriteAsJsonAsync(problemDetails, _jsonSerializerOptions);
        }
    }
}