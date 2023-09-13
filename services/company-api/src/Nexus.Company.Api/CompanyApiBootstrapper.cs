using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Nexus.CompanyAPI.Abstractions;
using Nexus.CompanyAPI.Data;
using Nexus.CompanyAPI.Data.Repositories;
using Nexus.CompanyAPI.Mapping;
using Nexus.CompanyAPI.Services;
using Nexus.CompanyAPI.Telemetry;
using Nexus.Framework.Web;
using OpenTelemetry.Resources;
using Steeltoe.Common.Http.Discovery;

namespace Nexus.CompanyAPI;

public class CompanyApiBootstrapper : NexusServiceBootstrapper
{
    public CompanyApiBootstrapper(string[] args) : base(args)
    {
    }

    protected override void AddServices()
    {
        base.AddServices();

        // Custom Meter for Metrics
        AppBuilder.Services.AddSingleton<ICompanyInstrumentation, CompanyInstrumentation>();
        
        AppBuilder.Services.AddOpenTelemetry()
            .ConfigureResource(c =>
            {
                c.AddService("company-api");
            })
            .WithMetrics(builder =>
            {
                builder.AddMeter(CompanyInstrumentation.MeterName);
            });
        
        // Internal Services
        AppBuilder.Services.AddScoped<ICompanyService, CompanyService>();
        AppBuilder.Services.AddScoped<ITagService, TagService>();
        
        AppBuilder.Services
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
        AppBuilder.Services.AddAutoMapper(typeof(CompanyProfile));
        AppBuilder.Services.AddValidatorsFromAssemblyContaining(typeof(Program));

        // Persistence
        AppBuilder.Services.AddNexusPersistence<ApplicationDbContext>(AppBuilder.Configuration);
        AppBuilder.Services.AddScoped<CompanyRepository>();
        AppBuilder.Services.AddScoped<TagRepository>();
        AppBuilder.Services.AddScoped<UnitOfWork>();
    }

    protected override void ConfigureMiddleware()
    {
        base.ConfigureMiddleware();
        
        using IServiceScope scope = App.Services.CreateScope();
        ApplicationDbContext db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();

        App.MapControllers();
    }
}