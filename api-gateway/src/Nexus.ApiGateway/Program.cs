using Nexus.ApiGateway.Extensions;
using Ocelot.DependencyInjection;

namespace Nexus.ApiGateway;

[ExcludeFromCodeCoverage]
public class Program
{
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
        services.RegisterDependencies();
    }
}