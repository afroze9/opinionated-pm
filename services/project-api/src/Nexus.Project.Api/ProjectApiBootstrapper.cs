using Nexus.Framework.Web;
using Nexus.ProjectAPI.Endpoints;

namespace Nexus.ProjectAPI;

public class ProjectApiBootstrapper : NexusServiceBootstrapper
{
    public ProjectApiBootstrapper(string[] args) : base(args)
    {
    }

    protected override void AddServices()
    {
        base.AddServices();
        AppBuilder.Services.AddOutputCache();
    }

    protected override void ConfigureMiddleware()
    {
        base.ConfigureMiddleware();
        App.UseOutputCache();
        App.AddProjectEndpoints();
        App.AddTodoEndpoints();
    }
}