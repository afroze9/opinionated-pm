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
using Quartz;
using Quartz.AspNetCore;

namespace Nexus.PeopleAPI;

public class ServiceBootstrapper : NexusServiceBootstrapper
{
    public ServiceBootstrapper(string[] args) : base(args)
    {
    }

    protected override void AddServices()
    {
        base.AddServices();
        // Internal Services
        AppBuilder.Services.AddSingleton<IPeopleInstrumentation, PeopleInstrumentation>();
        AppBuilder.Services.Configure<Auth0ManagementOptions>(AppBuilder.Configuration.GetSection("Auth0Management"));
        AppBuilder.Services.AddMemoryCache();
        
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
                Auth0ManagementOptions options = new Auth0ManagementOptions();//TODO
                AppBuilder.Configuration.GetSection("Auth0Management").Bind(options);

                httpClientOptions.BaseAddress = new Uri($"https://{options.Domain}");
            })
            .AddTypedClient<IIdentityService, Auth0IdentityService>();
        
        AppBuilder.Services.AddScoped<IPeopleService, PeopleService>();

        // Libraries
        AppBuilder.Services.AddAutoMapper(typeof(PeopleProfile));
        AppBuilder.Services.AddValidatorsFromAssemblyContaining(typeof(Program));
        AppBuilder.Services.AddQuartz(options =>
        {
            options.UseMicrosoftDependencyInjectionJobFactory();
            JobKey syncJobKey = new JobKey(nameof(PeopleSyncJob));
            options.AddJob<PeopleSyncJob>(o => o.WithIdentity(syncJobKey).DisallowConcurrentExecution());
            options.AddTrigger(triggerOptions =>
                triggerOptions
                    .ForJob(syncJobKey)
                    .WithIdentity("PeopleSyncJob-Trigger")
                    .WithSimpleSchedule(scheduleOptions =>
                        scheduleOptions
                            .WithIntervalInMinutes(10)
                            .RepeatForever()
                    )
            );
        });
        AppBuilder.Services.AddQuartzServer(options =>
        {
            options.WaitForJobsToComplete = true;
        });

        // Persistence
        AppBuilder.Services.AddNexusPersistence<ApplicationDbContext>(AppBuilder.Configuration);
        AppBuilder.Services.AddScoped<PeopleRepository>();
        AppBuilder.Services.AddScoped<SyncStatusRepository>();
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