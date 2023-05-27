using FluentValidation;
using ProjectManagement.CompanyAPI.Abstractions;
using ProjectManagement.CompanyAPI.Data;
using ProjectManagement.CompanyAPI.Data.Repositories;
using ProjectManagement.CompanyAPI.Mapping;
using ProjectManagement.CompanyAPI.Services;
using Steeltoe.Common.Http.Discovery;

namespace ProjectManagement.CompanyAPI.Extensions;

[ExcludeFromCodeCoverage]
public static class DependencyInjectionExtensions
{
    public static void RegisterDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        // Internal Services
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