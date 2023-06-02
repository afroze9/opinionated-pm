using Microsoft.Extensions.DependencyInjection.Extensions;
using Nexus.ApiGateway.Authorization;
using Nexus.ApiGateway.Extensions;
using Nexus.Framework.Web;
using Ocelot.Authorization;
using Ocelot.DependencyInjection;
using Ocelot.Provider.Consul;

namespace Nexus.ApiGateway;

public class ApiGatewayBootstrapper : Bootstrapper
{
    public ApiGatewayBootstrapper(string[] args) : base(args)
    {
    }

    protected override void AddServices()
    {
        base.AddServices();
        
        // Internal Services
        AppBuilder.Services
            .AddOcelot()
            .AddConsul();

        // The order matters here
        // Github Issue: https://github.com/ThreeMammals/Ocelot/issues/913
        AppBuilder.Services.RemoveAll<IScopesAuthorizer>();
        AppBuilder.Services.TryAddSingleton<IScopesAuthorizer, DelimitedScopesAuthorizer>();
    }

    protected override void AddConfiguration()
    {
        AppBuilder.Configuration.AddOcelot("Ocelot", AppBuilder.Environment);
        base.AddConfiguration();
    }

    protected override void ConfigureMiddleware()
    {
        // .NET
        App.UseCors("AllowAll");
        App.UseAuthentication();
        App.UseRouting();
        
        // Custom
        App.UseCustomOcelot().Wait();
    }
}