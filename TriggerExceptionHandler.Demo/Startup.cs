using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            services.AddMvc();
            services.TriggerInvalidModelStateResponse();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            app.UseTriggerExceptionHandler(nameof(TriggerExceptionHandler));

            app.UseMvc();
        }
    }
}
