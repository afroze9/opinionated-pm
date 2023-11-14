using System.Net;
using System.Net.Http.Json;
using System.Reflection;
using System.Security.Claims;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nexus.CompanyAPI.Controllers;
using Nexus.CompanyAPI.Data;
using Nexus.CompanyAPI.Data.Repositories;
using Nexus.CompanyAPI.DTO;
using Nexus.CompanyAPI.Entities;
using Nexus.CompanyAPI.Entities.Validations;
using Nexus.CompanyAPI.Mapping;
using Nexus.CompanyAPI.Model;
using Nexus.CompanyAPI.Services;
using Nexus.CompanyAPI.Telemetry;
using Nexus.Framework.Web.Services;
using Nexus.Persistence.Auditing;
using Nexus.SharedKernel.Contracts.Project;
using NSubstitute;
using Polly;
using Polly.Registry;

namespace Nexus.CompanyAPI.ComponentTests.Controllers;

[ExcludeFromCodeCoverage]
public class CompanyControllerTests : IDisposable
{
    private readonly HttpMessageHandler _httpMessageHandler;
    private readonly ApplicationDbContext _context;
    private readonly CompanyController _companyController;
    public CompanyControllerTests()
    {
        MapperConfiguration mapperConfiguration = new (cfg => { cfg.AddProfile(new CompanyProfile()); });
        IMapper mapper = mapperConfiguration.CreateMapper();
        
        _httpMessageHandler = Substitute.For<HttpMessageHandler>();
        HttpClient httpClient = new (_httpMessageHandler);
        
        ILogger<ProjectService> projectLogger = Substitute.For<ILogger<ProjectService>>();
        
        ResiliencePipelineProvider<string> resiliencePipelineProvider = Substitute.For<ResiliencePipelineProvider<string>>();
        resiliencePipelineProvider.GetPipeline<List<ProjectSummaryDto>>(Arg.Any<string>())
            .Returns(ResiliencePipeline<List<ProjectSummaryDto>>.Empty);
        
        ClaimsPrincipal claimsPrincipal = new (new ClaimsIdentity(new List<Claim>()
        {
            new ("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", "testuser"),
        }));
        HttpContext httpContext = Substitute.For<HttpContext>();
        httpContext.User.Returns(claimsPrincipal);
        
        HttpContextAccessor httpContextAccessor = Substitute.For<HttpContextAccessor>();
        CurrentUserService curentUserService = new (httpContextAccessor);
        DateTimeService dateTimeService = new ();
        
        AuditableEntitySaveChangesInterceptor auditableInterceptor = new (curentUserService, dateTimeService);
        DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new (options, auditableInterceptor);
        
        CompanyRepository companyRepository = new (_context);
        TagRepository tagRepository = new (_context);
        UnitOfWork unitOfWork = new (_context, companyRepository, tagRepository);

        ProjectService projectService = new (httpClient, mapper, projectLogger, resiliencePipelineProvider);
        ILogger<CompanyService>? companyLogger = Substitute.For<ILogger<CompanyService>>();

        CompanyValidator companyValidator = new ();

        CompanyService companyService = new (mapper, projectService, unitOfWork, companyLogger, companyValidator);
        CompanyInstrumentation companyInstrumentation = new ();
        _companyController = new (companyService, mapper, companyInstrumentation);
    }
    
    [Fact]
    public async Task GetAll_WhenCalled_ReturnsOkResult()
    {
        // Arrange
        List<Company> companies = new ()
        {
            new Company("Name 1"),
            new Company("Name 2"),
            new Company("Name 3"),
        };
        
        List<ProjectResponseModel> expectedProjects = new()
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
        
        await _context.Companies.AddRangeAsync(companies);
        await _context.SaveChangesAsync();
        
        Task<HttpResponseMessage> response = Task.FromResult<HttpResponseMessage>(new (HttpStatusCode.OK)
        {
            Content = JsonContent.Create(expectedProjects),
        });

        MethodInfo? method = _httpMessageHandler
            .GetType()
            .GetMethod("SendAsync", BindingFlags.NonPublic | BindingFlags.Instance);
        
        Assert.NotNull(method);
        
#pragma warning disable NS1000
#pragma warning disable NS1004
        method.Invoke(_httpMessageHandler, new object?[] { Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>() })
            .Returns(response);
#pragma warning restore NS1004
#pragma warning restore NS1000
        
        // Act
        IActionResult result = await _companyController.GetAll();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        List<CompanySummaryResponseModel>? resultCompanies =
            (result as OkObjectResult)!.Value as List<CompanySummaryResponseModel>;
        resultCompanies.Should().NotBeNull();
        resultCompanies.Should().HaveSameCount(companies);
    }


    [Fact]
    public async Task GetAll_WhenNoCompanies_ReturnsNotFound()
    {
        // Act
        IActionResult result = await _companyController.GetAll();

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetById_WhenCalled_ReturnsOkResult()
    {
        // Arrange
        List<Company> companies = new ()
        {
            new Company("Name 1"),
            new Company("Name 2"),
            new Company("Name 3"),
        };
        
        List<ProjectResponseModel> expectedProjects = new()
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
        
        await _context.Companies.AddRangeAsync(companies);
        await _context.SaveChangesAsync();
        
        Task<HttpResponseMessage> response = Task.FromResult<HttpResponseMessage>(new (HttpStatusCode.OK)
        {
            Content = JsonContent.Create(expectedProjects),
        });

        MethodInfo? method = _httpMessageHandler
            .GetType()
            .GetMethod("SendAsync", BindingFlags.NonPublic | BindingFlags.Instance);
        
        Assert.NotNull(method);
        
#pragma warning disable NS1000
#pragma warning disable NS1004
        method.Invoke(_httpMessageHandler, new object?[] { Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>() })
            .Returns(response);
#pragma warning restore NS1004
#pragma warning restore NS1000

        CompanyResponseModel expectedResult = new ()
        {
            Name = "Name 1",
            Id = 1,
            Projects = new List<ProjectSummaryResponseModel>()
            {
                new ()
                {
                    Name = "Project 1",
                    Id = 1,
                    CompanyId = 1,
                },
                new ()
                {
                    Name = "Project 2",
                    Id = 2,
                    CompanyId = 1,
                },
            }
        };
        
        // Act
        IActionResult result = await _companyController.GetById(1);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        CompanyResponseModel? resultCompany = (result as OkObjectResult)!.Value as CompanyResponseModel;
        resultCompany.Should().NotBeNull();
        resultCompany.Should().BeEquivalentTo(expectedResult);
    }
    
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _context.Database.EnsureDeleted();
        _context.Dispose();
        _httpMessageHandler.Dispose();
    }
}