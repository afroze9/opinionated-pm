using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProjectManagement.CompanyAPI.Abstractions;
using ProjectManagement.CompanyAPI.Controllers;
using ProjectManagement.CompanyAPI.DTO;
using ProjectManagement.CompanyAPI.Mapping;
using ProjectManagement.CompanyAPI.Model;

namespace ProjectManagement.CompanyAPI.UnitTests.Controllers;

[ExcludeFromCodeCoverage]
public class CompanyControllerTests
{
    private readonly CompanyController _companyController;
    private readonly Mock<IValidator<CompanyRequestModel>> _companyRequestModelValidatorMock = new ();
    private readonly Mock<ICompanyService> _companyServiceMock = new ();
    private readonly Mock<IValidator<CompanyUpdateRequestModel>> _companyUpdateRequestModelValidatorMock = new ();

    private readonly IMapper _mapper;

    public CompanyControllerTests()
    {
        MapperConfiguration mockMapper = new (cfg => { cfg.AddProfile(new CompanyProfile()); });
        _mapper = mockMapper.CreateMapper();
        _companyController = new CompanyController(_companyServiceMock.Object, _mapper,
            _companyRequestModelValidatorMock.Object, _companyUpdateRequestModelValidatorMock.Object);
    }

    [Fact]
    public async Task GetAll_WhenCalled_ReturnsOkResult()
    {
        // Arrange
        List<CompanySummaryDto> companies = new ()
        {
            new ()
            {
                Id = 1,
                Name = "Company 1",
            },
            new ()
            {
                Id = 2,
                Name = "Company 2",
            },
        };

        List<CompanySummaryResponseModel> expectedMappedCompanies =
            _mapper.Map<List<CompanySummaryResponseModel>>(companies);

        _companyServiceMock.Setup(x => x.GetAllAsync()).ReturnsAsync(companies);

        // Act
        ActionResult<List<CompanySummaryResponseModel>> result = await _companyController.GetAll();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        List<CompanySummaryResponseModel>? resultCompanies =
            (result.Result as OkObjectResult)!.Value as List<CompanySummaryResponseModel>;

        resultCompanies.Should().NotBeNull();
        resultCompanies.Should().HaveSameCount(expectedMappedCompanies);
        resultCompanies.Should().BeEquivalentTo(expectedMappedCompanies, options => options.ExcludingMissingMembers());
    }

    [Fact]
    public async Task GetAll_WhenNoCompanies_ReturnsNotFound()
    {
        // Arrange
        List<CompanySummaryDto> companies = new ();

        _companyServiceMock.Setup(x => x.GetAllAsync()).ReturnsAsync(companies);

        // Act
        ActionResult<List<CompanySummaryResponseModel>> result = await _companyController.GetAll();

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetById_WhenCalled_ReturnsOkResult()
    {
        // Arrange
        int id = 1;
        CompanyDto companyDto = new ()
        {
            Id = id,
            Name = "Company",
        };

        CompanyResponseModel expectedResponseModel = _mapper.Map<CompanyResponseModel>(companyDto);
        _companyServiceMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(companyDto);

        // Act
        ActionResult<CompanyResponseModel> result = await _companyController.GetById(id);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        CompanyResponseModel? resultCompany = (result.Result as OkObjectResult)!.Value as CompanyResponseModel;
        resultCompany.Should().NotBeNull();
        resultCompany.Should().BeEquivalentTo(expectedResponseModel);
    }

    [Fact]
    public async Task GetById_WhenCompanyNotFound_ReturnsNotFound()
    {
        // Arrange
        int id = 1;
        CompanyDto? companyDto = null;
        _companyServiceMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(companyDto);

        // Act
        ActionResult<CompanyResponseModel> result = await _companyController.GetById(id);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Create_WhenModelStateIsValid_ReturnsCreatedAtAction()
    {
        // Arrange
        CompanyRequestModel model = new ()
        {
            Name = "New Company",
        };

        ValidationResult validationResult = new ();
        _companyRequestModelValidatorMock.Setup(x => x.ValidateAsync(model, CancellationToken.None))
            .ReturnsAsync(validationResult);

        CompanySummaryDto companySummaryDto = new ()
        {
            Id = 1,
            Name = model.Name,
        };

        CompanySummaryDto createdCompanySummaryDto = new ()
        {
            Id = 1,
            Name = model.Name,
        };

        MapperConfiguration config = new (cfg => { cfg.AddProfile<CompanyProfile>(); });
        IMapper mapper = config.CreateMapper();

        Mock<IMapper> mapperMock = new ();
        mapperMock.Setup(x => x.Map<CompanySummaryDto>(model)).Returns(companySummaryDto);

        _companyServiceMock.Setup(x => x.CreateAsync(It.IsAny<CompanySummaryDto>()))
            .ReturnsAsync(createdCompanySummaryDto);

        CompanyResponseModel expectedResponseModel = mapper.Map<CompanyResponseModel>(createdCompanySummaryDto);

        // Act
        ActionResult<CompanyResponseModel> result = await _companyController.Create(model);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        CreatedAtActionResult? createdResult = result.Result as CreatedAtActionResult;
        createdResult.Should().NotBeNull();
        createdResult!.ActionName.Should().Be(nameof(CompanyController.GetById));
        createdResult.RouteValues.Should().ContainKey("id").And.ContainValue(createdCompanySummaryDto.Id);
        CompanyResponseModel? responseModel = createdResult.Value as CompanyResponseModel;
        responseModel.Should().BeEquivalentTo(expectedResponseModel);
    }

    [Fact]
    public async Task Create_WhenModelStateIsInvalid_ReturnsBadRequest()
    {
        // Arrange
        CompanyRequestModel model = new ()
        {
            Name = "New Company",
        };

        _companyRequestModelValidatorMock.Setup(x => x.ValidateAsync(model, CancellationToken.None)).ReturnsAsync(
            new ValidationResult(new List<ValidationFailure>
            {
                new ("Property", "Error message"),
            }));

        // Act
        ActionResult<CompanyResponseModel> result = await _companyController.Create(model);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        BadRequestObjectResult? badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.Value.Should().BeOfType<List<string>>();
    }

    [Fact]
    public async Task Update_WhenModelStateIsValid_ReturnsOkResult()
    {
        // Arrange
        int id = 1;
        CompanyUpdateRequestModel model = new ()
        {
            Id = id,
            Name = "Updated Company",
        };

        ValidationResult validationResult = new ();
        _companyUpdateRequestModelValidatorMock.Setup(x => x.ValidateAsync(model, CancellationToken.None))
            .ReturnsAsync(validationResult);

        CompanySummaryDto updatedCompanyDto = new ()
        {
            Id = id,
            Name = model.Name,
        };

        _companyServiceMock.Setup(x => x.UpdateNameAsync(id, model.Name)).ReturnsAsync(updatedCompanyDto);
        CompanyResponseModel expectedResponseModel = _mapper.Map<CompanyResponseModel>(updatedCompanyDto);

        // Act
        ActionResult<CompanyResponseModel> result = await _companyController.Update(id, model);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        OkObjectResult? okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        CompanyResponseModel? responseModel = okResult!.Value as CompanyResponseModel;
        responseModel.Should().BeEquivalentTo(expectedResponseModel);
    }

    [Fact]
    public async Task Update_WhenModelStateIsInvalid_ReturnsBadRequest()
    {
        // Arrange
        int id = 1;
        CompanyUpdateRequestModel model = new ()
        {
            Id = id,
            Name = "Updated Company",
        };

        _companyUpdateRequestModelValidatorMock.Setup(x => x.ValidateAsync(model, CancellationToken.None)).ReturnsAsync(
            new ValidationResult(new List<ValidationFailure>
            {
                new ("Property", "Error message"),
            }));

        // Act
        ActionResult<CompanyResponseModel> result = await _companyController.Update(id, model);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        ((BadRequestObjectResult) result!.Result!).Value.Should().BeOfType<List<string>>();
    }

    [Fact]
    public async Task Update_WhenCompanyNotFound_ReturnsBadRequest()
    {
        // Arrange
        int id = 1;
        CompanyUpdateRequestModel model = new ()
        {
            Id = 1,
            Name = "Updated Company",
        };

        CompanySummaryDto? updatedCompanyDto = null;
        _companyUpdateRequestModelValidatorMock.Setup(x => x.ValidateAsync(model, CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

        _companyServiceMock.Setup(x => x.UpdateNameAsync(id, model.Name)).ReturnsAsync(updatedCompanyDto);

        // Act
        ActionResult<CompanyResponseModel> result = await _companyController.Update(id, model);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        (result!.Result! as BadRequestObjectResult)!.Value.Should().Be($"Unable to find company with the id {id}");
    }

    [Fact]
    public async Task Delete_WhenCalled_ReturnsNoContent()
    {
        // Arrange
        int id = 1;
        _companyServiceMock.Setup(x => x.DeleteAsync(id)).Returns(Task.CompletedTask);

        // Act
        IActionResult result = await _companyController.Delete(id);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }
}