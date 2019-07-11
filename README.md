# TriggerExceptionHandler

*Super easy* **ASP.NET Core Exception Handler & ModelState validator**

## Startup.cs

```C#
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc();
    services.TriggerInvalidModelStateResponse();
}

public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    app.UseTriggerExceptionHandler(nameof(TriggerExceptionHandler));
    app.UseMvc();
}
```
