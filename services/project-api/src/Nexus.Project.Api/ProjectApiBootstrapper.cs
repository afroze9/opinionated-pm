using Nexus.Framework.Web;
using Nexus.ProjectAPI.Endpoints;

namespace Nexus.ProjectAPI;

public class ProjectApiBootstrapper : NexusServiceBootstrapper
{
    public ProjectApiBootstrapper(string[] args) : base(args)
    {
    }

    protected override void ConfigureMiddleware()
    {
        base.ConfigureMiddleware();
        App.AddProjectEndpoints();
        App.AddTodoEndpoints();
    }
}