using Microsoft.EntityFrameworkCore;
using PeopleAPI.Data;
using PeopleAPI.Extensions;

namespace PeopleAPI;

[ExcludeFromCodeCoverage]
public class Program
{
    public static void Main(string[] args)
    {
        CoreWebApplicationBuilder.BuildConfigureAndRun(
            args,
            configureDefaultMiddleware: true,
            preConfiguration: null,
            registerServices: (services, configuration, _) => { services.RegisterDependencies(configuration); },
            configureMiddleware: app =>
            {
                // DB Migration
                using IServiceScope scope = app.Services.CreateScope();
                ApplicationDbContext db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.Migrate();

                app.MapControllers();
            });
    }
}