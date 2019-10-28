# TriggerExceptionHandler

[![Codacy Badge](https://api.codacy.com/project/badge/Grade/577b53ee206c4c79a21e79494175f9b8)](https://app.codacy.com/app/TheTrigger/TriggerExceptionHandler?utm_source=github.com&utm_medium=referral&utm_content=TheTrigger/TriggerExceptionHandler&utm_campaign=Badge_Grade_Settings)

![Nuget](https://img.shields.io/nuget/dt/TriggerExceptionHandler.svg?label=NuGet%20Downloads&style=flat-square)

_Super easy_ **ASP.NET Core Exception Handler + ModelState validator** for Web API services.

- [ASP.NET Core 3.0](https://dotnet.microsoft.com/download/dotnet-core/3.0) ready
- Easy to use (just few lines)
- **Standard** models
  - https://tools.ietf.org/html/rfc7807
  - [ValidationProblemDetails model](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.validationproblemdetails?view=aspnetcore-2.2&viewFallbackFrom=viewFallbackFrom%3Daspnetcore-3.0)
- Based on `System.Text.Json`
- `ModelState.IsValid` to attribute: `[ValidateModelStateAttribute]`
- Thanks to [StrathWeb](https://www.strathweb.com/2018/07/centralized-exception-handling-and-request-validation-in-asp-net-core/) for the base pattern
- Compatible with [Sentry.io](https://sentry.io)

### Nuget package installation

```shell
Install-Package TriggerExceptionHandler
```

```shell
dotnet add package TriggerExceptionHandler
```

Some exceptions returns different `HttpStatusCode`:

- `UnauthorizedAccessException`: `Unauthorized (401)`
- `KeyNotFoundException`: `NotFound (404)`

Default is `500`.

# Setup

## Startup.cs

```C#
using TriggerExceptionHandler.Extensions;

public void ConfigureServices(IServiceCollection services)
{
    // services.AddMvc(); (you can still use AddMvc)
    // Please see: https://devblogs.microsoft.com/aspnet/asp-net-core-updates-in-net-core-3-0-preview-4/
    services.AddControllers();

    services.TriggerInvalidModelStateResponse();
}
```

```C#
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    // custom http status codes:
    // app.UseTriggerExceptionHandler(env.ApplicationName, exceptionsCode: new Ext2HttpCode { { typeof(ArgumentException), HttpStatusCode.Ambiguous } });

    app.UseTriggerExceptionHandler(env.ApplicationName);

    app.UseRouting();
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();

        // to catch any 404 route: (ofc you could use any Exception type)
        // KeyNotFoundException will be translated as 404 http status code
        endpoints.Map("{*url}", rd => throw new KeyNotFoundException($"Unable to find route: [{rd.Request.Method}] {rd.Request.Path}"));
    });
}
```

## Example responses

### Exception Model

```json
{
  "type": "ExpectedException",
  "title": "Your exception message",
  "status": 500,
  "detail": "...stack trace (if debugger is attached)",
  "instance": "urn:YourApplicationName:1299978476"
}
```

### `ValidateModelStateAttribute` model

```json
{
  "errors": {
    "Name": ["The Name field is required."],
    ...
  },
  "type": "ValidationProblemDetails",
  "title": "Request Validation Error",
  "status": 400,
  "detail": "One or more validation errors occurred",
  "instance": "urn:YourApplicationName:759630415"
}
```
