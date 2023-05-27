using FluentValidation;
using Nexus.ProjectAPI.Data;
using Nexus.ProjectAPI.Data.Repositories;
using Nexus.ProjectAPI.Mapping;

namespace Nexus.ProjectAPI.Extensions;

[ExcludeFromCodeCoverage]
public static class DependencyInjectionExtensions
{
    public static void RegisterDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        // Add Application Services if any

        // Libraries
        services.AddAutoMapper(typeof(ProjectProfile));
        services.AddValidatorsFromAssemblyContaining(typeof(Program));

        // Persistence
        services.AddCorePersistence<ApplicationDbContext>(configuration);
        services.AddScoped<ProjectRepository>();
        services.AddScoped<TodoRepository>();
        services.AddScoped<UnitOfWork>();
    }
}
