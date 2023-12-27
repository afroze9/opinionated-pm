using Nexus.Framework.Web;
using Nexus.PeopleAPI.Abstractions;
using Nexus.PeopleAPI.Configurations;
using Nexus.PeopleAPI.Services;
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

        AppBuilder.Services.Configure<Auth0ManagementOptions>(AppBuilder.Configuration.GetSection("Auth0Management"));
        AppBuilder.Services.AddMemoryCache();
        
        AppBuilder.Services
            .AddHttpClient("auth0_token")
            .ConfigureHttpClient(httpClientOptions =>
            {
                Auth0ManagementOptions options = new ();
                AppBuilder.Configuration.GetSection("Auth0Management").Bind(options);

                httpClientOptions.BaseAddress = new Uri($"https://{options.Domain}");
            })
            .AddTypedClient<IIdentityService, Auth0IdentityService>();
        
        AppBuilder.Services.AddQuartz(options =>
        {
            options.UseMicrosoftDependencyInjectionJobFactory();
            JobKey syncJobKey = new (nameof(PeopleSyncJob));
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
    }

    protected override void ConfigureMiddleware()
    {
        base.ConfigureMiddleware();
        App.MapControllers();
    }
}