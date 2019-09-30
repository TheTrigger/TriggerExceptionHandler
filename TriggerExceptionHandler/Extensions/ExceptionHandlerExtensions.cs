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

                var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
                var exception = errorFeature.Error;

                var eventId = new EventId(context.TraceIdentifier.GetHashCode(), nameof(Exception));

                logger?.LogError(eventId: eventId, exception: exception, message: exception.Message, context.Request);

                var problemDetails = new ProblemDetails
                {
                    Detail = showDetails ? exception.ToString() : null,
                    Instance = $"urn:{applicationName}:{eventId.Id.ToString()}",
                    Title = exception.Message,
                    Type = exception.GetType().Name,
                    Status = exceptionsCode[exception]
                };

                await context.Response.WriteJsonAsync(problemDetails);
            }));

            return app;
        }
    }
}