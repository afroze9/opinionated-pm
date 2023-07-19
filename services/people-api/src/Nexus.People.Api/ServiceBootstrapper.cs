using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Nexus.Framework.Web;
using Nexus.PeopleAPI.Abstractions;
using Nexus.PeopleAPI.Configurations;
using Nexus.PeopleAPI.Data;
using Nexus.PeopleAPI.Data.Repositories;
using Nexus.PeopleAPI.Mapping;
using Nexus.PeopleAPI.Services;
using Nexus.PeopleAPI.Telemetry;
using OpenTelemetry.Resources;

namespace Nexus.PeopleAPI;

public class ServiceBootstrapper : Bootstrapper
{
    public ServiceBootstrapper(string[] args) : base(args)
    {
    }

    protected override void AddServices()
    {
        base.AddServices();
        // Internal Services
        AppBuilder.Services.AddSingleton<IPeopleInstrumentation, PeopleInstrumentation>();
        
        // Custom Meter for Metrics
        AppBuilder.Services.AddOpenTelemetry()
            .ConfigureResource(c =>
            {
                c.AddService("people-api");
            })
            .WithMetrics(builder =>
            {
                builder.AddMeter(PeopleInstrumentation.MeterName);
            });
        
        AppBuilder.Services
            .AddHttpClient("auth0_token")
            .ConfigureHttpClient(httpClientOptions =>
            {
                Auth0ManagementOptions options = new Auth0ManagementOptions();
                AppBuilder.Configuration.GetSection("Auth0ManagementOptions").Bind(options);

                httpClientOptions.BaseAddress = new Uri($"https://{options.Domain}");
                httpClientOptions.DefaultRequestHeaders.Add("content-type", "application/x-www-form-urlencoded");
            })
            .AddTypedClient<IIdentityService, Auth0IdentityService>();
        
        AppBuilder.Services.AddScoped<IPeopleService, PeopleService>();

        // Libraries
        AppBuilder.Services.AddAutoMapper(typeof(PeopleProfile));
        AppBuilder.Services.AddValidatorsFromAssemblyContaining(typeof(Program));

        // Persistence
        AppBuilder.Services.AddCorePersistence<ApplicationDbContext>(AppBuilder.Configuration);
        AppBuilder.Services.AddScoped<PeopleRepository>();
        AppBuilder.Services.AddScoped<UnitOfWork>();
    }

    protected override void ConfigureMiddleware()
    {
        base.ConfigureMiddleware();
        
        // DB Migration
        using IServiceScope scope = App.Services.CreateScope();
        ApplicationDbContext db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();

        App.MapControllers();
    }
}