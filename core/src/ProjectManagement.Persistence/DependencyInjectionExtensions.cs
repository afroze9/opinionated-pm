using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectManagement.Persistence.Auditing;
using Steeltoe.Connector.PostgreSql;
using Steeltoe.Connector.PostgreSql.EFCore;

namespace ProjectManagement.Persistence;

public static class DependencyInjectionExtensions
{
    public static void AddCorePersistence<TContext>(this IServiceCollection services, IConfiguration configuration)
        where TContext : DbContext
    {
        services.AddDbContext<TContext>(options => { options.UseNpgsql(configuration); });
        services.AddScoped<AuditableEntitySaveChangesInterceptor>();
        services.AddPostgresHealthContributor(configuration);
    }
}