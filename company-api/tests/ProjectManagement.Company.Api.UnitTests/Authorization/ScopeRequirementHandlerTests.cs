using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using ProjectManagement.CompanyAPI.Authorization;

namespace ProjectManagement.CompanyAPI.UnitTests.Authorization;

[ExcludeFromCodeCoverage]
public class ScopeRequirementHandlerTests
{
    [Fact]
    public async Task Handler_WhenInvokedWithCorrectIdentityWithoutSpaces_Succeeds()
    {
        ScopeRequirement[] requirements = { new ("admin") };
        Claim[] claims = { new ("scope", "admin") };
        ClaimsIdentity identity = new (claims, "Basic");
        ClaimsPrincipal user = new (identity);
        AuthorizationHandlerContext context = new (requirements, user, null);

        ScopeRequirementHandler sut = new ();

        await sut.HandleAsync(context);

        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task Handler_WhenInvokedWithCorrectIdentityWithSpaces_Succeeds()
    {
        ScopeRequirement[] requirements = { new ("admin") };
        Claim[] claims = { new ("scope", "admin reader") };
        ClaimsIdentity identity = new (claims, "Basic");
        ClaimsPrincipal user = new (identity);
        AuthorizationHandlerContext context = new (requirements, user, null);

        ScopeRequirementHandler sut = new ();

        await sut.HandleAsync(context);

        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task Handler_WhenInvokedWithIncorrectIdentity_Fails()
    {
        ScopeRequirement[] requirements = { new ("admin") };
        Claim[] claims = { new ("scope", "reader") };
        ClaimsIdentity identity = new (claims, "Basic");
        ClaimsPrincipal user = new (identity);
        AuthorizationHandlerContext context = new (requirements, user, null);

        ScopeRequirementHandler sut = new ();

        await sut.HandleAsync(context);

        Assert.False(context.HasSucceeded);
    }
}