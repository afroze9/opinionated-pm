using Microsoft.Extensions.DependencyInjection.Extensions;
using Ocelot.Authorization;
using Ocelot.DependencyInjection;
using Ocelot.Provider.Consul;
using Nexus.ApiGateway.Authorization;

namespace Nexus.ApiGateway.Extensions;

[ExcludeFromCodeCoverage]
public static class DependencyInjectionExtensions
{
    public static void RegisterDependencies(this IServiceCollection services)
    {
        //services.AddGateway();
        
        // Internal Services
        services
            .AddOcelot()
            .AddConsul();

        // The order matters here
        // Github Issue: https://github.com/ThreeMammals/Ocelot/issues/913
        services.RemoveAll<IScopesAuthorizer>();
        services.TryAddSingleton<IScopesAuthorizer, DelimitedScopesAuthorizer>();
    }
}