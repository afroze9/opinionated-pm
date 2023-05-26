using System.Net;
using System.Net.Http.Json;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using ProjectManagement.CompanyAPI.DTO;
using ProjectManagement.CompanyAPI.Model;
using ProjectManagement.CompanyAPI.Services;

namespace ProjectManagement.CompanyAPI.UnitTests.Services;

[ExcludeFromCodeCoverage]
public class ProjectServiceTests
{
    private readonly HttpClient _client;
    private readonly IMapper _mapper;
    private readonly Mock<HttpMessageHandler> _mockHandler;
    private readonly Mock<ILogger<ProjectService>> _logger;

    public ProjectServiceTests()
    {
        MapperConfiguration config = new (cfg => { cfg.CreateMap<ProjectResponseModel, ProjectSummaryDto>(); });

        _mapper = config.CreateMapper();
        _mockHandler = new Mock<HttpMessageHandler>();
        _client = new HttpClient(_mockHandler.Object);
        _logger = new Mock<ILogger<ProjectService>>();
    }

    [Fact]
    public async Task GetProjectsByCompanyIdAsync_ReturnsProjects_WhenStatusCodeIsSuccess()
    {
        // Arrange
        int companyId = 1;
        List<ProjectResponseModel> expectedProjects = new List<ProjectResponseModel>
        {
            new ()
            {
                Id = 1,
                Name = "Project 1",
                CompanyId = 1,
            },
            new ()
            {
                Id = 2,
                Name = "Project 2",
                CompanyId = 1,
            },
        };

        HttpResponseMessage response = new (HttpStatusCode.OK)
        {
            Content = JsonContent.Create(expectedProjects),
        };

        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Get
                                                   && r.RequestUri ==
                                                   new Uri(
                                                       $"https://project-api/api/v1/Project?companyId={companyId}")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        ProjectService service = new (_client, _mapper, _logger.Object);

        // Act
        List<ProjectSummaryDto> actualProjects = await service.GetProjectsByCompanyIdAsync(companyId);

        // Assert
        Assert.Equal(expectedProjects.Count, actualProjects.Count);
    }

    [Fact]
    public async Task GetProjectsByCompanyIdAsync_ReturnsEmptyList_WhenStatusCodeIsNotSuccess()
    {
        // Arrange
        int companyId = 1;
        HttpResponseMessage response = new (HttpStatusCode.NotFound);

        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Get
                                                   && r.RequestUri ==
                                                   new Uri(
                                                       $"https://project-api/api/v1/Project?companyId={companyId}")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        ProjectService service = new (_client, _mapper, _logger.Object);

        // Act
        List<ProjectSummaryDto> actualProjects = await service.GetProjectsByCompanyIdAsync(companyId);

        // Assert
        Assert.Empty(actualProjects);
    }
}