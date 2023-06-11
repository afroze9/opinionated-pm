# Framework Details

## Application Bootstrapper
Currently, Nexus provides a default bootstrapper implementation for Web Applications (MVC APIs).
The default bootstrapper does the following:
* Sets up Configuration using appsettings.json, environment variables, and Consul KV
* Sets up Logging using Serilog and ElasticSearch
* Enables Swagger endpoints
* Enables Telemetry using Prometheus/Jaeger/Grafana
* Enables Service Discovery using Consul Discovery Server
* Enables Management/Actuator endnpoints similar to Spring Boot
* Adds custom Action Filters to time requests

You can check out the configuration [here](#configuration).

## Usage
To use the default Bootstrapper, run the following from `Program.cs`:
```csharp
public class Program
{
    public static void Main(string[] args)
    {
        Bootstrapper bootstrapper = new (args);
        bootstrapper.BootstrapAndRun();
    }
}
```

To customize the bootstrap process, create a new bootstrapper for your service, extend the provided Boostrapper, and
override the required functions:

```csharp
public class CompanyApiBootstrapper : Bootstrapper
{
    public CompanyApiBootstrapper(string[] args) : base(args)
    {
    }
}
```

The following can be customized:
* Configuration: Override this to setup custom configuration, e.g. add custom configuration providers/files etc.
* Logging: Override this to use custom logging providers/sinks
* Services: Override this to add application specific services to the Dependency Injection container. The best practice 
is to call `base.AddServices()` from the override to ensure all Framework Services are registered as well
* Middleware: Override this to use application specific Middleware and Middleware Order. You can check out the default
middleware order [here](#default-middleware-configuration)

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

## Default Middleware configuration
This flag enables/disables the configuration of default middleware. This step is done after the WebApplication has
been built and before it is run.

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