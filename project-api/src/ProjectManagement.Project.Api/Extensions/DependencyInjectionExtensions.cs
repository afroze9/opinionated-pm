using System.Reflection;
using FluentValidation;
using Microsoft.OpenApi.Models;
using ProjectManagement.ProjectAPI.Data;
using ProjectManagement.ProjectAPI.Data.Repositories;
using ProjectManagement.ProjectAPI.Mapping;
using Steeltoe.Discovery.Client;
using Steeltoe.Management.Endpoint;
using Steeltoe.Management.Endpoint.Health;
using Steeltoe.Management.Endpoint.Info;
using Steeltoe.Management.Endpoint.Refresh;

namespace ProjectManagement.ProjectAPI.Extensions;

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
                Title = "Project API",
                Description = "Project Microservice",
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
    
    private static void AddApplicationServices(this IServiceCollection services)
    {
    }

    private static void AddConsulDiscovery(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDiscoveryClient(configuration);
    }

    private static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCorePersistence<ApplicationDbContext>(configuration);
        services.AddScoped<ProjectRepository>();
        services.AddScoped<TodoRepository>();
        services.AddScoped<UnitOfWork>();
    }
    
    public static void RegisterDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddActuators(configuration);
        services.AddApiDocumentation();
        services.AddApplicationServices();
        services.AddAutoMapper(typeof(ProjectProfile));
        services.AddConsulDiscovery(configuration);
        services.AddControllers();
        services.AddMediatR(options => options.RegisterServicesFromAssembly(typeof(Program).Assembly));
        services.AddPersistence(configuration);
        services.AddCoreAuth(configuration, "project");
        services.AddCoreTelemetry(configuration);
        services.AddValidatorsFromAssemblyContaining(typeof(Program));

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy => { policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); });
        });
    }
}
