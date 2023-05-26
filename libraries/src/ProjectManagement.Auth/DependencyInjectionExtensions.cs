using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectManagement.Auth;

public static class DependencyInjectionExtensions
{
    private static readonly string[] Actions = { "read", "write", "update", "delete" };

    private static List<string> GetCrudPolicies(string resource)
    {
        return GetCrudPolicies(resource, Actions.ToList());
    }

    private static List<string> GetCrudPolicies(string resource, List<string> resourceActions)
    {
        return resourceActions.Select(action => $"{action}:{resource}").ToList();
    }

    private static void AddCrudPolicies(this AuthorizationOptions options, List<string> policies)
    {
        foreach (string policy in policies)
        {
            options.AddPolicy(policy, p => p.Requirements.Add(new ScopeRequirement(policy)));
        }
    }

    private static void AddCoreAuth(this IServiceCollection services, IConfiguration configuration,
        List<string> policies)
    {
        Auth0Settings auth0Settings = new ();
        configuration.GetRequiredSection(nameof(Auth0Settings)).Bind(auth0Settings);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.Authority = auth0Settings.Authority;
            options.Audience = auth0Settings.Audience;
        });

        services.AddSingleton<IAuthorizationHandler, ScopeRequirementHandler>();
        services.AddAuthorization(options => { options.AddCrudPolicies(policies); });
    }

    public static void AddCoreAuth(this IServiceCollection services, IConfiguration configuration, string resourceName)
    {
        List<string> policies = GetCrudPolicies(resourceName);
        services.AddCoreAuth(configuration, policies);
    }

    public static void AddCoreAuth(this IServiceCollection services, IConfiguration configuration, string resourceName,
        List<string> resourceActions)
    {
        List<string> policies = GetCrudPolicies(resourceName, resourceActions);
        services.AddCoreAuth(configuration, policies);
    }
}