using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Nexus.Framework.Web;
using Nexus.ProjectAPI.Data;
using Nexus.ProjectAPI.Data.Repositories;
using Nexus.ProjectAPI.Endpoints;
using Nexus.ProjectAPI.Mapping;

namespace Nexus.ProjectAPI;

public class ProjectApiBootstrapper : Bootstrapper
{
    public ProjectApiBootstrapper(string[] args) : base(args)
    {
    }

    protected override void AddServices()
    {
        base.AddServices();
        
        // Libraries
        AppBuilder.Services.AddAutoMapper(typeof(ProjectProfile));
        AppBuilder.Services.AddValidatorsFromAssemblyContaining(typeof(Program));

        // Persistence
        AppBuilder.Services.AddCorePersistence<ApplicationDbContext>(AppBuilder.Configuration);
        AppBuilder.Services.AddScoped<ProjectRepository>();
        AppBuilder.Services.AddScoped<TodoRepository>();
        AppBuilder.Services.AddScoped<UnitOfWork>();
    }

    protected override void ConfigureMiddleware()
    {
        base.ConfigureMiddleware();
        // DB Migration
        using IServiceScope scope = App.Services.CreateScope();
        ApplicationDbContext db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();

        App.AddProjectEndpoints();
        App.AddTodoEndpoints();
    }
}