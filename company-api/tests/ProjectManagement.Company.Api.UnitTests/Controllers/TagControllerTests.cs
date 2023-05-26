using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProjectManagement.CompanyAPI.Abstractions;
using ProjectManagement.CompanyAPI.Controllers;
using ProjectManagement.CompanyAPI.DTO;
using ProjectManagement.CompanyAPI.Model;

namespace ProjectManagement.CompanyAPI.UnitTests.Controllers;

[ExcludeFromCodeCoverage]
public class TagControllerTests
{
    private readonly Mock<ICompanyService> _companyServiceMock = new ();
    private readonly Mock<IMapper> _mapperMock = new ();

    private readonly TagController _sut;
    private readonly Mock<IValidator<TagRequestModel>> _tagRequestModelValidatorMock = new ();
    private readonly Mock<ITagService> _tagServiceMock = new ();

    public TagControllerTests()
    {
        _sut = new TagController(_mapperMock.Object, _companyServiceMock.Object, _tagServiceMock.Object,
            _tagRequestModelValidatorMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsListOfTagResponseModel_WhenCalled()
    {
        // Arrange
        List<TagDto> tags = new List<TagDto>
        {
            new ()
                { Name = "Tag1" },
            new ()
                { Name = "Tag2" },
        };

        List<TagResponseModel> tagResponseModels = new List<TagResponseModel>
        {
            new ()
                { Name = "Tag1" },
            new ()
                { Name = "Tag2" },
        };

        _tagServiceMock.Setup(x => x.GetAllAsync())
            .ReturnsAsync(tags);

        _mapperMock.Setup(x => x.Map<List<TagResponseModel>>(tags))
            .Returns(tagResponseModels);

        // Act
        ActionResult<List<TagResponseModel>> result = await _sut.GetAll();

        // Assert
        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result.Result);
        List<TagResponseModel> actual = Assert.IsType<List<TagResponseModel>>(okResult.Value);
        Assert.Equal(tagResponseModels.Count, actual.Count);

        for (int i = 0; i < tagResponseModels.Count; i++)
        {
            Assert.Equal(tagResponseModels[i].Name, actual[i].Name);
        }
    }

    [Fact]
    public async Task GetAll_ReturnsNotFound_WhenNoTagsExist()
    {
        // Arrange
        List<TagDto> tags = new ();

        _tagServiceMock.Setup(x => x.GetAllAsync())
            .ReturnsAsync(tags);

        // Act
        ActionResult<List<TagResponseModel>> result = await _sut.GetAll();

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenNameIsNull()
    {
        // Arrange
        string name = null;
        _tagRequestModelValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<TagRequestModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult
            {
                Errors = { new ValidationFailure("Name", "The Name field is required.") },
            });

        // Act
        ActionResult<TagResponseModel> result = await _sut.Create(name);

        // Assert
        BadRequestObjectResult badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        List<string> actual = Assert.IsType<List<string>>(badRequestResult.Value);
        Assert.Single(actual);
        Assert.Equal("The Name field is required.", actual.First());
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenValidationFails()
    {
        // Arrange
        string name = "tag";
        ValidationResult validationResult = new (new[] { new ValidationFailure("Name", "error") });

        _tagRequestModelValidatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<TagRequestModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        // Act
        ActionResult<TagResponseModel> result = await _sut.Create(name);

        // Assert
        BadRequestObjectResult badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        List<string> actual = Assert.IsType<List<string>>(badRequestResult.Value);
        Assert.Single(actual);
        Assert.Equal(validationResult.Errors.First().ErrorMessage, actual.First());
    }

    [Fact]
    public async Task Create_ReturnsCreatedTagResponseModel_WhenTagCreatedSuccessfully()
    {
        // Arrange
        string name = "tag";
        TagDto tagDto = new ()
            { Name = name };

        TagResponseModel tagResponseModel = new ()
            { Name = name };

        _tagRequestModelValidatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<TagRequestModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _tagServiceMock.Setup(x => x.CreateAsync(It.IsAny<string>()))
            .ReturnsAsync(tagDto);

        _mapperMock.Setup(x => x.Map<TagResponseModel>(tagDto))
            .Returns(tagResponseModel);

        // Act
        ActionResult<TagResponseModel> result = await _sut.Create(name);

        // Assert
        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result.Result);
        TagResponseModel actual = Assert.IsType<TagResponseModel>(okResult.Value);
        Assert.Equal(tagResponseModel.Name, actual.Name);
    }


    [Fact]
    public async Task Delete_ReturnsBadRequest_WhenNameIsNull()
    {
        // Arrange
        string name = null;

        // Act
        IActionResult result = await _sut.Delete(name);

        // Assert
        BadRequestObjectResult badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Tag could not be deleted", badRequestResult.Value);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenTagDeletedSuccessfully()
    {
        // Arrange
        string name = "tag";
        _tagServiceMock.Setup(x => x.DeleteAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act
        IActionResult result = await _sut.Delete(name);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsBadRequest_WhenTagCouldNotBeDeleted()
    {
        // Arrange
        string name = "tag";
        _tagServiceMock.Setup(x => x.DeleteAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act
        IActionResult result = await _sut.Delete(name);

        // Assert
        BadRequestObjectResult badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Tag could not be deleted", badRequestResult.Value);
    }

    [Fact]
    public async Task AddCompanyTag_ReturnsBadRequest_WhenNameIsNull()
    {
        // Arrange
        int id = 1;
        string name = null;

        _tagRequestModelValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<TagRequestModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult
            {
                Errors = { new ValidationFailure("Name", "The Name field is required.") },
            });

        // Act
        ActionResult<CompanyResponseModel> result = await _sut.AddCompanyTag(id, name);

        // Assert
        BadRequestObjectResult badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        List<string> actual = Assert.IsType<List<string>>(badRequestResult.Value);
        Assert.Single(actual);
        Assert.Equal("The Name field is required.", actual.First());
    }

    [Fact]
    public async Task AddCompanyTag_ReturnsBadRequest_WhenValidationFails()
    {
        // Arrange
        int id = 1;
        string name = "tag";
        ValidationResult validationResult = new (new[] { new ValidationFailure("Name", "error") });

        _tagRequestModelValidatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<TagRequestModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        // Act
        ActionResult<CompanyResponseModel> result = await _sut.AddCompanyTag(id, name);

        // Assert
        BadRequestObjectResult badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        List<string> actual = Assert.IsType<List<string>>(badRequestResult.Value);
        Assert.Single(actual);
        Assert.Equal(validationResult.Errors.First().ErrorMessage, actual.First());
    }

    [Fact]
    public async Task AddCompanyTag_ReturnsBadRequest_WhenCompanyNotFound()
    {
        // Arrange
        int id = 1;
        string name = "tag";
        ValidationResult validationResult = new ();
        CompanySummaryDto updatedCompany = null;

        _tagRequestModelValidatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<TagRequestModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        _companyServiceMock.Setup(x => x.AddTagAsync(It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync(updatedCompany);

        // Act
        ActionResult<CompanyResponseModel> result = await _sut.AddCompanyTag(id, name);

        // Assert
        BadRequestObjectResult badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        string actual = Assert.IsType<string>(badRequestResult.Value);
        Assert.Equal($"Unable to find company with the id {id}", actual);
    }

    [Fact]
    public async Task AddCompanyTag_ReturnsUpdatedCompanyResponseModel_WhenTagAddedSuccessfully()
    {
        // Arrange
        int id = 1;
        string name = "tag";
        string companyName = "company";
        ValidationResult validationResult = new ();
        CompanySummaryDto companySummaryDto = new ()
            { Id = id, Name = companyName };

        CompanyResponseModel companyResponseModel = new ()
            { Id = id, Name = companyName };

        _tagRequestModelValidatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<TagRequestModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        _companyServiceMock.Setup(x => x.AddTagAsync(It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync(companySummaryDto);

        _mapperMock.Setup(x => x.Map<CompanyResponseModel>(companySummaryDto))
            .Returns(companyResponseModel);

        // Act
        ActionResult<CompanyResponseModel> result = await _sut.AddCompanyTag(id, name);

        // Assert
        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result.Result);
        CompanyResponseModel actual = Assert.IsType<CompanyResponseModel>(okResult.Value);
        Assert.Equal(companyResponseModel.Id, actual.Id);
    }

    [Fact]
    public async Task DeleteCompanyTag_ReturnsBadRequest_WhenCompanyNotFound()
    {
        // Arrange
        int id = 1;
        string name = "tag";
        CompanySummaryDto updatedCompany = null;

        _companyServiceMock.Setup(x => x.DeleteTagAsync(It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync(updatedCompany);

        // Act
        ActionResult<CompanyResponseModel> result = await _sut.DeleteCompanyTag(id, name);

        // Assert
        BadRequestObjectResult badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        string actual = Assert.IsType<string>(badRequestResult.Value);
        Assert.Equal($"Unable to find company with the id {id}", actual);
    }

    [Fact]
    public async Task DeleteCompanyTag_ReturnsNoContent_WhenTagDeletedSuccessfully()
    {
        // Arrange
        int id = 1;
        string name = "tag";
        string companyName = "company";
        CompanySummaryDto companySummaryDto = new ()
            { Id = id, Name = companyName };

        _companyServiceMock.Setup(x => x.DeleteTagAsync(It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync(companySummaryDto);

        // Act
        ActionResult<CompanyResponseModel> result = await _sut.DeleteCompanyTag(id, name);

        // Assert
        Assert.IsType<NoContentResult>(result.Result);
    }
}