using Microsoft.EntityFrameworkCore;
using Nexus.HealthChecksDashboard.Abstractions;
using Nexus.HealthChecksDashboard.Configuration;
using Nexus.HealthChecksDashboard.Data;
using Nexus.HealthChecksDashboard.Services;
using Steeltoe.Common.Http.Discovery;
using Steeltoe.Discovery.Client;

namespace Nexus.HealthChecksDashboard.Extensions;

[ExcludeFromCodeCoverage]
public static class DependencyInjectionExtensions
{
    private static void AddConsulDiscovery(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDiscoveryClient(configuration);

        services.AddHttpContextAccessor();
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
    
    private static void AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<HealthCheckOptions>(configuration.GetSection("HealthCheck"));
        services.AddScoped<IHealthChecksService, HealthChecksService>();
        services.AddHostedService<HealthCheckBackgroundService>();
    }
    
    public static void RegisterDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddConsulDiscovery(configuration);
        services.AddHealthChecks(configuration);
        services.AddRazorPages();
        services.AddServerSideBlazor();

        services.AddScoped<IHealthRecordService, HealthRecordService>();

        services.AddDbContext<ApplicationDbContext>(options => { options.UseInMemoryDatabase("HealthCheckDb"); });
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy => { policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); });
        });
    }
}
