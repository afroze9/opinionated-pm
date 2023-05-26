using Ocelot.DependencyInjection;
using ProjectManagement.Configuration;

namespace ProjectManagement.ApiGateway.Extensions;

[ExcludeFromCodeCoverage]
public static class ConfigurationExtensions
{
    public static void AddApplicationConfiguration(
        this ConfigurationManager configuration,
        IWebHostEnvironment environment)
    {
        configuration.AddCoreConfiguration();
        configuration.AddOcelot("Ocelot", environment);
    }
}