using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace TriggerExceptionHandler.Extensions;

public static class ExceptionHandlerExtensions
{
    /// <summary>
    /// Adds a middleware to the pipeline that will catch exceptions, log them, and retrieve a standard response
    /// </summary>
    public static IApplicationBuilder UseTriggerExceptionHandler(this IApplicationBuilder app, TriggerExceptionService triggerExceptionHandler)
    {
        app.UseExceptionHandler(configure => configure.Use(triggerExceptionHandler.HandleExceptionAndWriteResponse));
        return app;
    }

    public static IServiceCollection AddTriggerExceptionHandler(this IServiceCollection services) // TODO: add configure
    {
        services.AddSingleton<TriggerExceptionService>();

        services.AddScoped<ValidationProblemDetailsResult>();
        services.TriggerInvalidModelStateResponse();

        return services;
    }
}

