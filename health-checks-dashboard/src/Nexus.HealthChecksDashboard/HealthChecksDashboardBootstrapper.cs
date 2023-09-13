using Microsoft.EntityFrameworkCore;
using Nexus.Framework.Web;
using Nexus.HealthChecksDashboard.Abstractions;
using Nexus.HealthChecksDashboard.Configuration;
using Nexus.HealthChecksDashboard.Data;
using Nexus.HealthChecksDashboard.Services;
using Steeltoe.Common.Http.Discovery;

namespace Nexus.HealthChecksDashboard;

public class HealthChecksDashboardBootstrapper : NexusServiceBootstrapper
{
    public HealthChecksDashboardBootstrapper(string[] args) : base(args)
    {
    }

    protected override void AddServices()
    {
        base.AddServices();
        // Internal Services
        AppBuilder.Services.Configure<HealthCheckOptions>(AppBuilder.Configuration.GetSection("HealthCheck"));
        AppBuilder.Services.AddScoped<IHealthChecksService, HealthChecksService>();
        AppBuilder.Services.AddScoped<IHealthRecordService, HealthRecordService>();
        AppBuilder.Services.AddHostedService<HealthCheckBackgroundService>();
        AppBuilder.Services.AddRazorPages();
        AppBuilder.Services.AddServerSideBlazor();
        AppBuilder.Services.AddDbContext<ApplicationDbContext>(options => { options.UseInMemoryDatabase("HealthCheckDb"); });
        
        AppBuilder.Services
            .AddHttpClient("healthchecks")
            .AddServiceDiscovery()
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler
                {
                    ClientCertificateOptions = ClientCertificateOption.Manual,
                    ServerCertificateCustomValidationCallback = (_, _, _, _) => true,
                };
            });
        //AppBuilder.Services.RegisterDependencies(AppBuilder.Configuration);
    }

    protected override void ConfigureMiddleware()
    {
        if (!App.Environment.IsDevelopment())
        {
            App.UseExceptionHandler("/Error");
            App.UseHsts();
        }
        
        App.UseHttpsRedirection();
        App.UseStaticFiles();
        App.UseRouting();
        App.MapBlazorHub();
        App.MapFallbackToPage("/_Host");
    }
}