﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace TriggerExceptionHandler.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection TriggerInvalidModelStateResponse(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = 
                    context => context.HttpContext.RequestServices.GetRequiredService<ValidationProblemDetailsResult>();
            });

            return services;
        }
    }
}