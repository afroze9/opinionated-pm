using Nexus.Auth;

namespace Nexus.ProjectAPI.UnitTests.Authorization;

[ExcludeFromCodeCoverage]
public class ScopeRequirementTests
{
    [Fact]
    public async Task TestScopeRequirement()
    {
        // Arrange
        string scope = "test";

        // Act
        ScopeRequirement sut = new (scope);

        // Assert
        Assert.Equal(scope, sut.Scope);
    }
}