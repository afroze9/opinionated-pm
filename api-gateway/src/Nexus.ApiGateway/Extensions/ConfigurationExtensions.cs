using Ocelot.DependencyInjection;
using Nexus.Configuration;

namespace Nexus.ApiGateway.Extensions;

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