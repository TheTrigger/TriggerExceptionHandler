using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace TriggerExceptionHandler;

public class TriggerExceptionHandler
{
    private readonly IOptions<JsonOptions> _jsonOptions;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public TriggerExceptionHandler(IOptions<JsonOptions> jsonOptions)
    {
        _jsonOptions = jsonOptions;
        _jsonSerializerOptions = _jsonOptions.Value.JsonSerializerOptions;
    }

    public Task HandleExceptionAndWriteResponse(HttpContext httpContext, Func<Task> next)
    {
        if (httpContext is null) throw new ArgumentNullException(nameof(httpContext));

        var showDetails = Debugger.IsAttached;

        var errorFeature = httpContext.Features.Get<IExceptionHandlerPathFeature>();
        var exception = errorFeature?.Error ?? new Exception("Unknown error");
        if (exception is AggregateException aggregateException)
            exception = aggregateException.Flatten();

        var exceptionName = exception.GetType().Name;

        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        var statusCode = exception switch
        {
            UnauthorizedAccessException => HttpStatusCode.Unauthorized,
            KeyNotFoundException or FileNotFoundException => HttpStatusCode.NotFound,
            _ => HttpStatusCode.InternalServerError,
        };

        var problemDetails = new ProblemDetails
        {
            Title = exception.Message, // exception.TargetSite.Name,
            Type = exceptionName ?? nameof(Exception),
            Detail = showDetails ? exception.Demystify().ToString() : null,
            Instance = $"{Environment.MachineName}:error:{traceId}",
            Status = (int)statusCode,
        };

        if (!string.IsNullOrEmpty(exception.HelpLink))
        {
            try
            {
                var locationUri = new Uri(exception.HelpLink);
                httpContext.Response.Headers.Add("Location", locationUri.ToString());
            }
            catch (Exception) { }
        }

        httpContext.Response.StatusCode = problemDetails.Status.Value;
        return httpContext.Response.WriteAsJsonAsync(problemDetails, options: _jsonSerializerOptions);
    }
}