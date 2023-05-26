using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectManagement.Auth;

public static class DependencyInjectionExtensions
{
    private static readonly string[] Actions = { "read", "write", "update", "delete" };
    
    private static void AddCrudPolicies(this AuthorizationOptions options, string resource)
    {
        foreach (string action in Actions)
        {
            options.AddPolicy($"{action}:{resource}",
                policy => policy.Requirements.Add(new ScopeRequirement($"{action}:{resource}")));
        }
    }
    
    // TODO: This can be extended to add more scopes/policies
    public static void AddAuth(this IServiceCollection services, IConfiguration configuration, string resourceName)
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

        services.AddAuthorization(options => { options.AddCrudPolicies(resourceName); });
        services.AddSingleton<IAuthorizationHandler, ScopeRequirementHandler>();
    }
}