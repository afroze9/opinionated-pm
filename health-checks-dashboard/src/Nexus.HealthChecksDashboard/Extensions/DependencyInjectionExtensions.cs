using Microsoft.EntityFrameworkCore;
using Nexus.HealthChecksDashboard.Abstractions;
using Nexus.HealthChecksDashboard.Configuration;
using Nexus.HealthChecksDashboard.Data;
using Nexus.HealthChecksDashboard.Services;
using Steeltoe.Common.Http.Discovery;

namespace Nexus.HealthChecksDashboard.Extensions;

[ExcludeFromCodeCoverage]
public static class DependencyInjectionExtensions
{
    public static void RegisterDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        // Internal Services
        services.Configure<HealthCheckOptions>(configuration.GetSection("HealthCheck"));
        services.AddScoped<IHealthChecksService, HealthChecksService>();
        services.AddScoped<IHealthRecordService, HealthRecordService>();
        services.AddHostedService<HealthCheckBackgroundService>();
        services.AddRazorPages();
        services.AddServerSideBlazor();
        services.AddDbContext<ApplicationDbContext>(options => { options.UseInMemoryDatabase("HealthCheckDb"); });
        
        services
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
    }
}
