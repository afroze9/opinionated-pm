using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Nexus.Configuration;
using Nexus.Logging;
using Serilog;

namespace Microsoft.Extensions.DependencyInjection;

public static class CoreWebApplicationBuilder
{
    public static WebApplication BuildAndConfigure(string[] args,
        Action<IServiceCollection, IConfiguration> registerServices,
        Action<WebApplication> configure)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        builder.Configuration.AddCoreConfiguration();
        builder.Logging.AddCoreLogging(builder.Configuration);
        builder.Services.AddWebFramework(builder.Configuration);

        // Register app specific services
        registerServices(builder.Services, builder.Configuration);

        WebApplication app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseCors("AllowAll");
        app.UseAuthentication();
        app.UseAuthorization();

        // Run app specific configuration
        configure(app);

        return app;
    }

    public static void BuildConfigureAndRun(string[] args,
        Action<IServiceCollection, IConfiguration> registerServices,
        Action<WebApplication> configure)
    {
        try
        {
            WebApplication app = BuildAndConfigure(args, registerServices, configure);
            app.Run();
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}