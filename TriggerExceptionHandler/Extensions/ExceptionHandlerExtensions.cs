using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

namespace TriggerExceptionHandler.Extensions
{
    public static class ExceptionHandlerExtensions
    {
        /// <summary>
        /// Adds a middleware to the pipeline that will catch exceptions, log them, and retrieve a standard response
        /// </summary>
        public static IApplicationBuilder UseTriggerExceptionHandler(this IApplicationBuilder app, string applicationName, ILogger logger = null, Ext2HttpCode exceptionsCode = null)
        {
            if (applicationName == null) throw new ArgumentNullException(nameof(applicationName));

            if (exceptionsCode == null)
                exceptionsCode = new Ext2HttpCode();

            app.UseExceptionHandler(a => a.Run(async context =>
            {
                var showDetails = Debugger.IsAttached;

                // ExceptionHandlerFeature
                var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
                var exception = errorFeature.Error;

                var exceptionName = exception.GetType().Name;
                var eventId = new EventId(context.TraceIdentifier.GetHashCode(), exceptionName);

                var problemDetails = new ProblemDetails
                {
                    Detail = showDetails ? exception.ToString() : null,
                    Instance = $"urn:{applicationName}:{eventId.Id.ToString()}",
                    Title = exception.Message,
                    Type = exceptionName,
                    Status = exceptionsCode[exception]
                };

                logger?.LogError(eventId: eventId, exception: exception, message: exception.Message, context.Request);

                context.Response.StatusCode = problemDetails.Status.Value;
                await context.Response.WriteJsonAsync(problemDetails).ConfigureAwait(false);
            }));

            return app;
        }
    }
}