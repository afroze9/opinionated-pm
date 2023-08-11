using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using LanguageExt.Common;
using Microsoft.AspNetCore.Mvc;
using Nexus.CompanyAPI.Abstractions;
using Nexus.CompanyAPI.Controllers;
using Nexus.CompanyAPI.DTO;
using Nexus.CompanyAPI.Entities;
using Nexus.CompanyAPI.Exceptions;
using Nexus.CompanyAPI.Mapping;
using Nexus.CompanyAPI.Model;
using NSubstitute;

namespace Nexus.CompanyAPI.UnitTests.Controllers;

[ExcludeFromCodeCoverage]
public class CompanyControllerTests
{
    private readonly CompanyController _companyController;
    private readonly IValidator<CompanyRequestModel> _companyRequestModelValidatorMock = Substitute.For<IValidator<CompanyRequestModel>>();
    private readonly IValidator<Company> _companyValidatorMock = Substitute.For<IValidator<Company>>();
    private readonly IValidator<CompanyUpdateRequestModel> _companyUpdateRequestModelValidatorMock = Substitute.For<IValidator<CompanyUpdateRequestModel>>();
    private readonly ICompanyService _companyServiceMock = Substitute.For<ICompanyService>();
    private readonly IMapper _mapper;

    public CompanyControllerTests()
    {
        MapperConfiguration mockMapper = new (cfg => { cfg.AddProfile(new CompanyProfile()); });

        _mapper = mockMapper.CreateMapper();
        _companyController = new CompanyController(
            _companyServiceMock, _mapper,
            new TestCompanyInstrumentation());
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

        _companyServiceMock.GetAllAsync().Returns(companies);
        
        // Act
        IActionResult result = await _companyController.GetAll();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        List<CompanySummaryResponseModel>? resultCompanies =
            (result as OkObjectResult)!.Value as List<CompanySummaryResponseModel>;

        resultCompanies.Should().NotBeNull();
        resultCompanies.Should().HaveSameCount(expectedMappedCompanies);
        resultCompanies.Should().BeEquivalentTo(expectedMappedCompanies, options => options.ExcludingMissingMembers());
    }

    [Fact]
    public async Task GetAll_WhenNoCompanies_ReturnsNotFound()
    {
        // Arrange
        _companyServiceMock.GetAllAsync().Returns(new Result<List<CompanySummaryDto>>(new CompanyNotFoundException(-1)));

        // Act
        IActionResult result = await _companyController.GetAll();

        // Assert
        result.Should().BeOfType<NotFoundResult>();
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
        _companyServiceMock.GetByIdAsync(Arg.Any<int>()).Returns(companyDto);

        // Act
        IActionResult result = await _companyController.GetById(id);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        CompanyResponseModel? resultCompany = (result as OkObjectResult)!.Value as CompanyResponseModel;
        resultCompany.Should().NotBeNull();
        resultCompany.Should().BeEquivalentTo(expectedResponseModel);
    }

    [Fact]
    public async Task GetById_WhenCompanyNotFound_ReturnsNotFound()
    {
        // Arrange
        const int id = 1;
        _companyServiceMock.GetByIdAsync(Arg.Any<int>())
            .Returns(new Result<CompanyDto>(new CompanyNotFoundException(id)));

        // Act
        IActionResult result = await _companyController.GetById(id);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
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
        _companyRequestModelValidatorMock
            .ValidateAsync(model, Arg.Any<CancellationToken>())
            .Returns(validationResult);

        CompanySummaryDto companySummaryDto = new ()
        {
            Id = 1,
            Name = model.Name,
        };

        Company createdCompanySummaryDto = new (model.Name)
        {
             Id = 1,
        };

        MapperConfiguration config = new (cfg => { cfg.AddProfile<CompanyProfile>(); });
        IMapper mapper = config.CreateMapper();

        var mapperMock = Substitute.For<IMapper>();
        mapperMock.Map<CompanySummaryDto>(model).Returns(companySummaryDto);

        _companyServiceMock.CreateAsync(Arg.Any<Company>())
            .Returns(createdCompanySummaryDto);

        CompanyResponseModel expectedResponseModel = mapper.Map<CompanyResponseModel>(createdCompanySummaryDto);

        // Act
        IActionResult result = await _companyController.Create(model);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        CreatedAtActionResult? createdResult = result as CreatedAtActionResult;
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
        string name = "New Company";
        Company company = new (name);
        CompanyRequestModel model = new()
        {
            Name = name,
        };

        _companyValidatorMock.ValidateAsync(company, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(new List<ValidationFailure>
            {
                new ("Property", "Error message"),
            }));

        List<ValidationFailure> validationErrors = new()
        {
            new ("Property", "Error message"),
        };

        _companyServiceMock.CreateAsync(Arg.Any<Company>())
            .Returns(new Result<Company>(new ValidationException(validationErrors)));
        
        // Act
        IActionResult result = await _companyController.Create(model);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        BadRequestObjectResult? badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.Value.Should().BeOfType<ValidationException>();
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
        _companyUpdateRequestModelValidatorMock.ValidateAsync(model, Arg.Any<CancellationToken>())
            .Returns(validationResult);

        Company updatedCompanyDto = new (model.Name)
        {
            Id = id,
        };

        _companyServiceMock.UpdateNameAsync(id, model.Name)
            .Returns(updatedCompanyDto);
        CompanyResponseModel expectedResponseModel = _mapper.Map<CompanyResponseModel>(updatedCompanyDto);

        // Act
        IActionResult result = await _companyController.Update(id, model);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        OkObjectResult? okResult = result as OkObjectResult;
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
            Id = 1,
            Name = "Updated Company",
        };

        _companyUpdateRequestModelValidatorMock.ValidateAsync(model, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _companyServiceMock.UpdateNameAsync(id, model.Name)
            .Returns(new Result<Company>(new ValidationException(new List<ValidationFailure>())));

        // Act
        IActionResult result = await _companyController.Update(id, model);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        (result as BadRequestObjectResult)!.Value.Should().BeOfType<ValidationException>();
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

        _companyUpdateRequestModelValidatorMock.ValidateAsync(model, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _companyServiceMock.UpdateNameAsync(id, model.Name)
            .Returns(new Result<Company>(new CompanyNotFoundException(id)));

        // Act
        IActionResult result = await _companyController.Update(id, model);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        (result as BadRequestObjectResult)!.Value.Should().BeOfType<CompanyNotFoundException>();
    }

    [Fact]
    public async Task Delete_WhenCalled_ReturnsNoContent()
    {
        // Arrange
        int id = 1;

        _companyServiceMock.DeleteAsync(id)
            .Returns(true);

        // Act
        IActionResult result = await _companyController.Delete(id);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }
}