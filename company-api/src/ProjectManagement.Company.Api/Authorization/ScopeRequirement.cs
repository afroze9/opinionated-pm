using Microsoft.AspNetCore.Authorization;

namespace ProjectManagement.CompanyAPI.Authorization;

/// <summary>
///     Represents an authorization requirement that is satisfied when the user has the specified scope.
/// </summary>
[ExcludeFromCodeCoverage]
public class ScopeRequirement : IAuthorizationRequirement
{
    /// <summary>
    ///     Initializes a new instance of the ScopeRequirement class with the specified scope.
    /// </summary>
    /// <param name="scope">The required scope for this authorization requirement.</param>
    public ScopeRequirement(string scope)
    {
        Scope = scope;
    }

    /// <summary>
    ///     Gets the required scope for this authorization requirement.
    /// </summary>
    public string Scope { get; }
}