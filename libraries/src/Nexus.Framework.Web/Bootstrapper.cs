using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nexus.Configuration;
using Nexus.Logs;
using Serilog;

namespace Nexus.Framework.Web;

public abstract class Bootstrapper
{
    protected string[] Args;

    protected readonly WebApplicationBuilder AppBuilder;

    protected WebApplication App = null!;

    protected Bootstrapper(string[] args)
    {
        Args = args;
        AppBuilder = WebApplication.CreateBuilder(args);
    }
    
    public void BootstrapAndRun()
    {
        try
        {
            AddConfiguration();
            AddLogging();
            AddServices();
            App = AppBuilder.Build();
            ConfigureMiddleware();
            App.Run();
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    protected virtual void AddConfiguration()
    {
        AppBuilder.Configuration.AddCoreConfiguration();
    }

    protected virtual void AddLogging()
    {
        AppBuilder.Logging.AddCoreLogging(AppBuilder.Configuration);
    }

    protected virtual void AddServices()
    {
        AppBuilder.Services.AddWebFramework(AppBuilder.Configuration);
    }

    protected virtual void ConfigureMiddleware()
    {
        // This is default middleware order
        if (App.Environment.IsDevelopment())
        {
            App.UseSwagger();
            App.UseSwaggerUI();
        }

        App.UseHttpsRedirection();
        App.UseCors("AllowAll");
        App.UseAuthentication();
        App.UseAuthorization();
        
        // TODO: Can probably read config, get frameworksettings, check if telemetry is enabled 
        App.UseOpenTelemetryPrometheusScrapingEndpoint();
    }
}

