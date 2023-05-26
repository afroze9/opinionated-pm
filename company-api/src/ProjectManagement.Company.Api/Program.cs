using Microsoft.EntityFrameworkCore;
using ProjectManagement.CompanyAPI.Data;
using ProjectManagement.CompanyAPI.Extensions;
using ProjectManagement.Configuration;
using ProjectManagement.Logging;
using Serilog;

namespace ProjectManagement.CompanyAPI;

[ExcludeFromCodeCoverage]
public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        builder.Configuration.AddCoreConfiguration();
        builder.Logging.AddCoreLogging(builder.Configuration);
        builder.Services.RegisterDependencies(builder.Configuration);

        WebApplication app = builder.Build();
        ApplyMigration(app);
        app.Configure().Run();

        Log.CloseAndFlush();
    }

    private static void ApplyMigration(WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        ApplicationDbContext db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
    }
}

[ExcludeFromCodeCoverage]
public static class AppConfigurationExtensions
{
    public static WebApplication Configure(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseCors("AllowAll");
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        return app;
    }
}