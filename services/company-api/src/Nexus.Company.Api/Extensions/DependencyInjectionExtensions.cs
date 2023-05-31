using FluentValidation;
using Nexus.CompanyAPI.Abstractions;
using Nexus.CompanyAPI.Data;
using Nexus.CompanyAPI.Data.Repositories;
using Nexus.CompanyAPI.Mapping;
using Nexus.CompanyAPI.Services;
using Nexus.CompanyAPI.Telemetry;
using OpenTelemetry.Resources;
using Steeltoe.Common.Http.Discovery;

namespace Nexus.CompanyAPI.Extensions;

[ExcludeFromCodeCoverage]
public static class DependencyInjectionExtensions
{
    public static void RegisterDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        // Internal Services
        services.AddSingleton<ICompanyInstrumentation, CompanyInstrumentation>();
        
        // Custom Meter for Metrics
        services.AddOpenTelemetry()
            .ConfigureResource(c =>
            {
                c.AddService("company-api");
            })
            .WithMetrics(builder =>
            {
                builder.AddMeter(CompanyInstrumentation.MeterName);
            });
        
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<ITagService, TagService>();
        services
            .AddHttpClient("projects")
            .AddServiceDiscovery()
            .ConfigureHttpClient((serviceProvider, options) =>
            {
                IHttpContextAccessor httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();

                if (httpContextAccessor.HttpContext == null)
                {
                    return;
                }

                string? bearerToken = httpContextAccessor.HttpContext.Request.Headers["Authorization"]
                    .FirstOrDefault(h =>
                        !string.IsNullOrEmpty(h) &&
                        h.StartsWith("bearer ", StringComparison.InvariantCultureIgnoreCase));

                if (!string.IsNullOrEmpty(bearerToken))
                {
                    options.DefaultRequestHeaders.Add("Authorization", bearerToken);
                }
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler
                {
                    ClientCertificateOptions = ClientCertificateOption.Manual,
                    ServerCertificateCustomValidationCallback = (_, _, _, _) => true,
                };
            })
            .AddTypedClient<IProjectService, ProjectService>();

        // Libraries
        services.AddAutoMapper(typeof(CompanyProfile));
        services.AddValidatorsFromAssemblyContaining(typeof(Program));

        // Persistence
        services.AddCorePersistence<ApplicationDbContext>(configuration);
        services.AddScoped<CompanyRepository>();
        services.AddScoped<TagRepository>();
        services.AddScoped<UnitOfWork>();
    }
}