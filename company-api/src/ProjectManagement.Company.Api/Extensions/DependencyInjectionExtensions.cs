using System.Reflection;
using FluentValidation;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using ProjectManagement.Auth;
using ProjectManagement.CompanyAPI.Abstractions;
using ProjectManagement.CompanyAPI.Data;
using ProjectManagement.CompanyAPI.Filters;
using ProjectManagement.CompanyAPI.Mapping;
using ProjectManagement.CompanyAPI.Services;
using ProjectManagement.Configuration;
using ProjectManagement.Persistence;
using ProjectManagement.Persistence.Abstractions;
using ProjectManagement.Persistence.Auditing;
using Steeltoe.Common.Http.Discovery;
using Steeltoe.Connector.PostgreSql;
using Steeltoe.Connector.PostgreSql.EFCore;
using Steeltoe.Discovery.Client;
using Steeltoe.Management.Endpoint;
using Steeltoe.Management.Endpoint.Health;
using Steeltoe.Management.Endpoint.Info;
using Steeltoe.Management.Endpoint.Refresh;

namespace ProjectManagement.CompanyAPI.Extensions;

[ExcludeFromCodeCoverage]
public static class DependencyInjectionExtensions
{
    private static readonly string[] Actions = { "read", "write", "update", "delete" };

    private static void AddActuators(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthActuator(configuration);
        services.AddInfoActuator(configuration);
        services.AddHealthChecks();
        services.AddRefreshActuator();
        services.ActivateActuatorEndpoints();
    }
    
    private static void AddApiDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Company API",
                Description = "Company Microservice",
            });

            string xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName));

            options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "JWT Authorization header using the Bearer scheme.",
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "bearer",
                        },
                    },
                    Array.Empty<string>()
                },
            });
        });
    }
    
    private static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<ITagService, TagService>();
        services.AddSingleton<ICurrentUserService, CurrentUserService>();
        services.AddSingleton<IDateTime, DateTimeService>();
    }
    
    private static void AddConsulDiscovery(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDiscoveryClient(configuration);

        services.AddHttpContextAccessor();
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
    }

    private static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped(typeof(IRepository<>), typeof(EfRepositoryBase<>));
        services.AddScoped(typeof(IReadRepository<>), typeof(EfRepositoryBase<>));
        services.AddDbContext<ApplicationDbContext>(options => { options.UseNpgsql(configuration); });
        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        services.AddPostgresHealthContributor(configuration);
    }

    private static void AddTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        TelemetrySettings telemetrySettings = new ();
        configuration.GetRequiredSection(nameof(TelemetrySettings)).Bind(telemetrySettings);

        services
            .AddOpenTelemetry()
            .WithTracing(builder =>
                {
                    builder
                        .AddHttpClientInstrumentation()
                        .AddAspNetCoreInstrumentation()
                        .ConfigureResource(options =>
                        {
                            options.AddService(
                                telemetrySettings.ServiceName,
                                serviceVersion: telemetrySettings.ServiceVersion,
                                autoGenerateServiceInstanceId: true);
                        })
                        .AddOtlpExporter(options => { options.Endpoint = new Uri(telemetrySettings.Endpoint); });

                    if (telemetrySettings.EnableConsoleExporter)
                    {
                        builder.AddConsoleExporter();
                    }

                    if (telemetrySettings.EnableAlwaysOnSampler)
                    {
                        builder.SetSampler<AlwaysOnSampler>();
                    }
                    else
                    {
                        builder.SetSampler(new TraceIdRatioBasedSampler(telemetrySettings.SampleProbability));
                    }
                }
            )
            .WithMetrics(builder =>
            {
                builder
                    .AddAspNetCoreInstrumentation()
                    .AddOtlpExporter(options => { options.Endpoint = new Uri(telemetrySettings.Endpoint); });
            });
    }
    
    public static void RegisterDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddActuators(configuration);
        services.AddApiDocumentation();
        services.AddApplicationServices();
        services.AddAutoMapper(typeof(CompanyProfile));
        services.AddConsulDiscovery(configuration);
        services.AddControllers(options => { options.Filters.Add<LoggingFilter>(); });
        services.AddMediatR(options => options.RegisterServicesFromAssembly(typeof(Program).Assembly));
        services.AddPersistence(configuration);
        services.AddAuth(configuration, "company");
        services.AddTelemetry(configuration);
        services.AddValidatorsFromAssemblyContaining(typeof(Program));

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy => { policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); });
        });
    }
}