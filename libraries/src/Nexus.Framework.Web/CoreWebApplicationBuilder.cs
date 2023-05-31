using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Nexus.Configuration;
using Nexus.Logging;
using Serilog;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class CoreWebApplicationBuilder
{
    private static WebApplication BuildDefaultApp(
        string[] args,
        Action<ConfigurationManager, IWebHostEnvironment>? preConfiguration,
        Action<IServiceCollection, IConfiguration, IWebHostEnvironment>? registerServices)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        
        // Configuration
        preConfiguration?.Invoke(builder.Configuration, builder.Environment);
        builder.Configuration.AddCoreConfiguration();
        
        // Logging
        builder.Logging.AddCoreLogging(builder.Configuration);
        
        // Core Services
        builder.Services.AddWebFramework(builder.Configuration);

        // Register app specific services
        registerServices?.Invoke(builder.Services, builder.Configuration, builder.Environment);

        return builder.Build();
    }

    private static void ConfigureDefaultMiddleware(this WebApplication app)
    {
        // This is default middleware order
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseCors("AllowAll");
        app.UseAuthentication();
        app.UseAuthorization();
        
        // TODO: find a way to call this from Tracing Library
        app.UseOpenTelemetryPrometheusScrapingEndpoint();
    }

    public static void BuildConfigureAndRun(
        string[] args,
        bool configureDefaultMiddleware,
        Action<ConfigurationManager, IWebHostEnvironment>? preConfiguration,
        Action<IServiceCollection, IConfiguration, IWebHostEnvironment>? registerServices,
        Action<WebApplication>? configureMiddleware)
    {
        try
        {
            WebApplication app = BuildDefaultApp(args, preConfiguration, registerServices);
            if (configureDefaultMiddleware)
            {
                app.ConfigureDefaultMiddleware();
            }

            configureMiddleware?.Invoke(app);

            app.Run();
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}