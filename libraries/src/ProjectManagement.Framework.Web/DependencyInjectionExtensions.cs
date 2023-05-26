using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using ProjectManagement.Framework.Web.Configuration;
using ProjectManagement.Framework.Web.Filters;
using ProjectManagement.Framework.Web.Services;
using ProjectManagement.Persistence.Auditing;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions
{
    private static void AddApiDocumentation(this IServiceCollection services, ApiDocs settings)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = settings.Version,
                Title = settings.Title,
                Description = settings.Description,
            });

            Assembly? entryAssembly = Assembly.GetEntryAssembly();

            if (entryAssembly != null)
            {
                string xmlFileName = $"{entryAssembly.GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName));
            }

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

    public static void AddWebFramework(this IServiceCollection services, IConfiguration configuration)
    {
        FrameworkSettings settings = new ();
        configuration.GetRequiredSection(nameof(FrameworkSettings)).Bind(settings);

        if (settings.Swagger is { Enable: true })
        {
            services.AddApiDocumentation(settings.Swagger);
        }

        services.AddControllers(options =>
        {
            if (settings.Filters is { EnableActionLogging: true })
            {
                options.Filters.Add<LoggingFilter>();
            }
        });
        
        if (settings.Telemetry is { Enable: true })
        {
            services.AddCoreTelemetry(configuration);
        }

        services.AddCoreAuth(configuration, "company");
        services.AddSingleton<ICurrentUserService, CurrentUserService>();
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy => { policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); });
        });
    }
}