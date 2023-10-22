using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Nexus.CompanyAPI.Abstractions;
using Nexus.CompanyAPI.Data;
using Nexus.CompanyAPI.Mapping;
using Nexus.CompanyAPI.Services;
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
        
        // Libraries
        AppBuilder.Services.AddAutoMapper(typeof(CompanyProfile));
        AppBuilder.Services.AddValidatorsFromAssemblyContaining(typeof(Program));
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