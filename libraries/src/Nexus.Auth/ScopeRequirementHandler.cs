using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Nexus.Auth;

/// <summary>
///     Provides a handler for the ScopeRequirement authorization requirement.
/// </summary>
public class ScopeRequirementHandler : AuthorizationHandler<ScopeRequirement>
{
    /// <inheritdoc />
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ScopeRequirement requirement)
    {
        // Searches for the "scope" claim in the user's claims and checks if the required scope is among the values of that claim.
        Claim? scopeClaim =
            context.User.Claims.FirstOrDefault(c => string.Equals(c.Type, "scope", StringComparison.OrdinalIgnoreCase));

        if (scopeClaim is not null && scopeClaim.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Any(c => string.Equals(c, requirement.Scope, StringComparison.OrdinalIgnoreCase)))
        {
            // The user has the required scope, so the requirement is satisfied.
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}