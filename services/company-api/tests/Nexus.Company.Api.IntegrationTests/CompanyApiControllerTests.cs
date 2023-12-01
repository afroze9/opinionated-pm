using System.Net;
using System.Security.Claims;

namespace Nexus.CompanyAPI.IntegrationTests;

[ExcludeFromCodeCoverage]
public class CompanyApiControllerTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly TestWebApplicationFactory<Program> _factory;

    public CompanyApiControllerTests(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Api_WhenCalledWithoutAuthorization_ReturnsUnauthorized()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();
        
        // Act
        HttpResponseMessage response = await client.GetAsync("api/v1/Company");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task Api_WhenCalledWithoutAuthorization_ReturnsForbidden()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();
        
        List<Claim> claims = new ()
        {
            new Claim(ClaimTypes.Name, "testuser"),
        };
        
        string token = claims.GetToken();
        client.AddAuthorizationTokenHeader(token);
        
        // Act
        HttpResponseMessage response = await client.GetAsync("api/v1/Company");
        
        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
    
    [Fact]
    public async Task Api_WhenCalledWithoutAuthorization_ReturnsOk()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();
        
        List<Claim> claims = new ()
        {
            new Claim(ClaimTypes.Name, "testuser"),
            new Claim("scope", "read:company"),
        };
        
        string token = claims.GetToken();
        client.AddAuthorizationTokenHeader(token);
        
        // Act
        HttpResponseMessage response = await client.GetAsync("api/v1/Company");
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}