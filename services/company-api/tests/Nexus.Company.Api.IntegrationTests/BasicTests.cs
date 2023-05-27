namespace Nexus.CompanyAPI.IntegrationTests;

//[ExcludeFromCodeCoverage]
//public class BasicTests : IClassFixture<TestWebApplicationFactory<Program>>
//{
//    private readonly TestWebApplicationFactory<Program> _factory;

//    public BasicTests(TestWebApplicationFactory<Program> factory)
//    {
//        _factory = factory;
//    }

//    [Fact]
//    public async Task Api_WhenCalledWithoutAuthorization_ReturnsUnauthorized()
//    {
//        // Arrange
//        HttpClient client = _factory.CreateClient();

//        // Act
//        HttpResponseMessage response = await client.GetAsync("api/v1/Company");

//        // Assert
//        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
//    }
//}