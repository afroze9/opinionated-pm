using Microsoft.EntityFrameworkCore;
using Nexus.CompanyAPI.Abstractions;
using Nexus.CompanyAPI.Data;
using Nexus.CompanyAPI.Services;
using Nexus.CompanyAPI.Telemetry;
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

        AppBuilder.Services.AddNexusTelemetry(AppBuilder.Configuration);
        AppBuilder.Services.AddNexusMeters("company-api", new[] { CompanyInstrumentation.MeterName });
        AppBuilder.Services.AddNexusTypedClient<IProjectService, ProjectService>(AppBuilder.Configuration, "projects");
        AppBuilder.Services.AddNexusPersistence<ApplicationDbContext>(AppBuilder.Configuration);
    }

    protected override void ConfigureMiddleware()
    {
        base.ConfigureMiddleware();
        
        using IServiceScope scope = App.Services.CreateScope();
        ApplicationDbContext db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();

        App.MapControllers();
    }
}