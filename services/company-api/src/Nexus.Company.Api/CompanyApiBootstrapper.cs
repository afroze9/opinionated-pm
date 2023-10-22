using Nexus.Framework.Web;
namespace Nexus.CompanyAPI;

public class CompanyApiBootstrapper : NexusServiceBootstrapper
{
    public CompanyApiBootstrapper(string[] args) : base(args)
    {
    }

    protected override void ConfigureMiddleware()
    {
        base.ConfigureMiddleware();
        App.MapControllers();
    }
}