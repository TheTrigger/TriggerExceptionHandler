using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TriggerExceptionHandler.Extensions
{
    public static class ExceptionHandlerExtensions
    {
        /// <summary>
        /// Adds a middleware to the pipeline that will catch exceptions, log them, and retrieve a standard response
        /// </summary>
        public static IApplicationBuilder UseTriggerExceptionHandler(this IApplicationBuilder app,
            string applicationName, ILogger logger = null, Ext2HttpCode exceptionsCode = null
            )
        {
            var he = new HandleException(applicationName, logger, exceptionsCode);
            return app.UseExceptionHandler(new ExceptionHandlerOptions
            {
                ExceptionHandler = he.HandleRequest,
            });
        }


        class HandleException
        {
            private static string _applicationName;
            private static ILogger _logger;
            private static Ext2HttpCode _exceptionsCode;

            public HandleException(string applicationName, ILogger logger, Ext2HttpCode exceptionsCode)
            {
                _applicationName = applicationName;
                _logger = logger;
                _exceptionsCode = exceptionsCode ?? new Ext2HttpCode();
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
                    Status = _exceptionsCode[exception]
                };


                // log the exception etc..
                context.Response.StatusCode = problemDetails.Status.Value;
                context.Response.WriteJson(problemDetails, "application/problem+json");

                return Task.CompletedTask;
            };
        }
    }
}
