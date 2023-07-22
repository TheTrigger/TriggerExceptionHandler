using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json.Serialization;
using TriggerExceptionHandler.Extensions;

namespace TriggerExceptionHandler.Demo
{
    /// <summary>
    /// https://www.strathweb.com/2018/07/centralized-exception-handling-and-request-validation-in-asp-net-core/
    /// </summary>
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddMvc();
            services.AddControllers();
            services.AddTriggerExceptionHandler();

            services.AddControllers().AddControllersAsServices().AddJsonOptions(options =>
            {
#if DEBUG
                options.JsonSerializerOptions.WriteIndented = true;
#endif
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.AllowTrailingCommas = true;
                options.JsonSerializerOptions.ReadCommentHandling = System.Text.Json.JsonCommentHandling.Skip;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(TriggerExceptionService exceptionHandler, IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseTriggerExceptionHandler(exceptionHandler);

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.Map("{*url}", rd => throw new KeyNotFoundException($"Unable to find route: [{rd.Request.Method}] {rd.Request.Path}"));
            });
        }
    }
}