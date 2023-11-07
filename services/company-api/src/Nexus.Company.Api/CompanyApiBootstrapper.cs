using Nexus.CompanyAPI.Resilience;
using Nexus.Framework.Web;

namespace Nexus.CompanyAPI;

public class CompanyApiBootstrapper : NexusServiceBootstrapper
{
    public CompanyApiBootstrapper(string[] args) : base(args)
    {
    }

    protected override void AddServices()
    {
        base.AddServices();
        AppBuilder.Services.AddResilience(AppBuilder.Configuration);
    }
    
    protected override void ConfigureMiddleware()
    {
        base.ConfigureMiddleware();
        App.MapControllers();
    }
}