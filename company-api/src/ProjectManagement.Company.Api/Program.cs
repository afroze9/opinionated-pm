using Microsoft.EntityFrameworkCore;
using ProjectManagement.CompanyAPI.Data;
using ProjectManagement.CompanyAPI.Extensions;

namespace ProjectManagement.CompanyAPI;

[ExcludeFromCodeCoverage]
public class Program
{
    public static void Main(string[] args)
    {
        CoreWebApplicationBuilder.BuildConfigureAndRun(args,
            (services, configuration) => { services.RegisterDependencies(configuration); },
            app =>
            {
                // DB Migration
                using IServiceScope scope = app.Services.CreateScope();
                ApplicationDbContext db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.Migrate();

                app.MapControllers();
            });
    }
}