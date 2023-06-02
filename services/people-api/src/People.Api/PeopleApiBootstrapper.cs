using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Nexus.Framework.Web;
using OpenTelemetry.Resources;
using PeopleAPI.Abstractions;
using PeopleAPI.Data;
using PeopleAPI.Data.Repositories;
using PeopleAPI.Mapping;
using PeopleAPI.Services;
using PeopleAPI.Telemetry;

namespace PeopleAPI;

public class PeopleApiBootstrapper : Bootstrapper
{
    public PeopleApiBootstrapper(string[] args) : base(args)
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