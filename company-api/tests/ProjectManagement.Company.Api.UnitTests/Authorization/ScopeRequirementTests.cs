using ProjectManagement.CompanyAPI.Authorization;

namespace ProjectManagement.CompanyAPI.UnitTests.Authorization;

[ExcludeFromCodeCoverage]
public class ScopeRequirementTests
{
    [Fact]
    public void Constructor_WhenCalled_SetsScope()
    {
        ScopeRequirement sut = new ("scope");

        Assert.Equal("scope", sut.Scope);
    }
}