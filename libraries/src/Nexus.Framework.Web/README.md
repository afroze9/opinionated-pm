# How to Use

## Configuration

Ensure the following settings exist in appsettings.json / Environment variables / Consul KV:

```json
{
  "FrameworkSettings": {
    "Auth": {
      "Enable": true,
      "ResourceName": "<api-resource-name>"
    },
    "Swagger": {
      "Enable": true,
      "Version": "v1",
      "Title": "<service-display-name>",
      "Description": "<service-description>"
    },
    "Filters": {
      "EnableActionLogging": true
    },
    "Telemetry": {
      "Enable": true
    },
    "Management": {
      "Enable": true
    },
    "Discovery": {
      "Enable": true
    }
  }
}
```

## Usage

```csharp
public static void Main(string[] args)
{
    CoreWebApplicationBuilder.BuildConfigureAndRun(
        args,
        configureDefaultMiddleware: false,
        preConfiguration: PreConfiguration,
        registerServices: RegisterServices,
        configureMiddleware: ConfigureMiddleware);
}

private static void ConfigureMiddleware(WebApplication app)
{
    app.UseCors("AllowAll");
    app.UseAuthentication();
    app.UseRouting();
    app.UseCustomOcelot().Wait();
}

private static void PreConfiguration(ConfigurationManager configurationManager, IWebHostEnvironment environment)
{
    configurationManager.AddOcelot("Ocelot", environment);
}

private static void RegisterServices(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
{
    services
        .AddOcelot()
        .AddConsul();
    
    services.RemoveAll<IScopesAuthorizer>();
    services.TryAddSingleton<IScopesAuthorizer, DelimitedScopesAuthorizer>();
}
```

### ConfigureDefaultMiddleware
This flag enables/disables the configuration of default middleware. This step is done after the WebApplication has been built and before it is run.

By default it configures this middleware pipeline (good for controller based APIs):
```csharp
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
```

### PreConfiguration
This action adds configuration while the app is being built. You can use it to add custom configurations.
In the example above, we are using it to combine `ocelot.global.json`, `ocelot.company.api.json`, and `ocelot.project.api.json` into `ocelot.json` and adding it to the configuration pipeline.

### RegisterServices
This action is used to add application specific services.

### ConfigureMiddleware
This action is used to configure custom middleware pipeline

The order of operations:
* WebApplicationBuilder is created
* `preConfiguration` is invoked if passed an action
* CoreConfiguration is added from `Nexus.Configuration`
* CoreLogging is added from `Nexus.Logging`
* CoreServices are added if enabled:
  * Swagger Documentation
  * Action Filters (LoggingFilter as of now)
  * CoreTelemetry from `Nexus.Telemetry`
  * CoreActuators from `Nexus.Management`
  * Discovery Client for Consul
  * CoreAuth from `Nexus.Auth`
* `registerServices` is invoked if passed an action
* WebApplication is built
* Default Middleware is configured if `configureDefaultMiddleware` is enabled
* `configureMiddleware` is invoked if passed an action
* WebApplication is run