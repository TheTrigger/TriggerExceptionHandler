# TriggerExceptionHandler

*Super easy* **ASP.NET Core Exception Handler & ModelState validator** for MVC services

Some exceptions returns different `HttpStatusCode`:
- `UnauthorizedAccessException`: `Unauthorized (401)`
- `KeyNotFoundException`: `NotFound (404)`

## TODO list
- [x] ILogger
- [ ] Custom Dictionary<Type(Exception), HttpStatusCode>
- [ ] Custom serializer
- [ ] Conditional details
- [ ] Extend status codes

## Startup.cs

```C#
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc();
    services.TriggerInvalidModelStateResponse();
}

public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    app.UseTriggerExceptionHandler("ApplicationName");
    app.UseMvc();
}
```

## Example responses

### Exceptions
```json
{
    "type": "Exception",
    "title": "Expected exception message",
    "status": 500,
    "detail": "...stack trace (if debugger is attached)",
    "instance": "urn:ApplicationName:1299978476"
}
```

### Dto Models

```json
{
    "errors": {
        "Name": [
            "The Name field is required."
        ]
    },
    "type": "ValidationProblemDetails",
    "title": "Request Validation Error",
    "status": 400,
    "detail": "One or more validation errors occurred",
    "instance": "urn:ApplicationName:759630415"
}
```
