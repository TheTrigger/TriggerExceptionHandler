using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace TriggerExceptionHandler.Extensions
{
    public static class ExceptionHandlerExtensions
    {
        public static IApplicationBuilder UseTriggerExceptionHandler(this IApplicationBuilder app,
            string applicationName,
            ILogger logger = null)
        {
            var he = new HandleException(applicationName, logger);
            return app.UseExceptionHandler(new ExceptionHandlerOptions
            {
                ExceptionHandler = he.HandleRequest,
            });
        }

        class HandleException
        {
            private static string _applicationName;
            private static ILogger _logger;

            public HandleException(string applicationName, ILogger logger)
            {
                _applicationName = applicationName;
                _logger = logger;
            }

            public RequestDelegate HandleRequest = (context) =>
            {
                bool showDetails = Debugger.IsAttached;

                var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
                var exception = errorFeature.Error;

                var eventId = new EventId(context.TraceIdentifier.GetHashCode(), nameof(Exception));
                _logger?.LogError(
                    eventId: eventId,
                    exception: exception,
                    message: exception.Message,

                    context.Request);

                var problemDetails = new ProblemDetails
                {
                    Detail = showDetails ? exception.ToString() : null,
                    Instance = $"urn:{_applicationName}:{eventId.Id}",
                    Title = exception.Message,
                    Type = exception.GetType().Name,
                };


                switch (exception)
                {
                    case UnauthorizedAccessException unauthorizedAccess:
                        problemDetails.Status = (int)HttpStatusCode.Unauthorized;
                        break;

                    case KeyNotFoundException keyNotFound:
                        problemDetails.Status = (int)HttpStatusCode.NotFound;
                        break;

                    /*case DbUpdateException dbUpdate:
                        problemDetails.Status = (int)HttpStatusCode.Conflict;
                        break;*/

                    default:
                        problemDetails.Status = (int)HttpStatusCode.InternalServerError;
                        break;
                }

                // log the exception etc..
                context.Response.StatusCode = problemDetails.Status.Value;
                context.Response.WriteJson(problemDetails, "application/problem+json");

                return Task.CompletedTask;
            };
        }
    }
}
