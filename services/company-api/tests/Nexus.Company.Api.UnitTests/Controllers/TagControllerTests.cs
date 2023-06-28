using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using LanguageExt.Common;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Nexus.CompanyAPI.Abstractions;
using Nexus.CompanyAPI.Controllers;
using Nexus.CompanyAPI.DTO;
using Nexus.CompanyAPI.Entities;
using Nexus.CompanyAPI.Exceptions;
using Nexus.CompanyAPI.Model;

namespace Nexus.CompanyAPI.UnitTests.Controllers;

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
        List<Tag> tags = new()
        {
            new ("Tag1"),
            new ("Tag2"),
        };

        List<TagResponseModel> tagResponseModels = new()
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
        List<Tag> tags = new ();

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
        string name = null!;
        _tagRequestModelValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<TagRequestModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult
            {
                Errors = { new ValidationFailure("Name", "The Name field is required.") },
            });

        _tagServiceMock.Setup(x => x.CreateAsync(It.IsAny<string>()))
            .ReturnsAsync(new Result<Tag>(new ValidationException(new List<ValidationFailure>()
            {
                new ValidationFailure("Name", "The Name field is required."),
            })));

        // Act
        IActionResult result = await _sut.Create(name);

        // Assert
        BadRequestObjectResult badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var actual = Assert.IsType<ValidationException>(badRequestResult.Value);
        Assert.Single(actual.Errors);
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
        
        _tagServiceMock.Setup(x => x.CreateAsync(It.IsAny<string>()))
            .ReturnsAsync(new Result<Tag>(new ValidationException(new List<ValidationFailure>()
            {
                new ValidationFailure("Name", "The Name field is required."),
            })));

        // Act
        IActionResult result = await _sut.Create(name);

        // Assert
        BadRequestObjectResult badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var actual = Assert.IsType<ValidationException>(badRequestResult.Value);
        Assert.Single(actual.Errors);
    }

    [Fact]
    public async Task Create_ReturnsCreatedTagResponseModel_WhenTagCreatedSuccessfully()
    {
        // Arrange
        string name = "tag";
        Tag tag = new (name);

        TagResponseModel tagResponseModel = new ()
            { Name = name };

        _tagRequestModelValidatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<TagRequestModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _tagServiceMock.Setup(x => x.CreateAsync(It.IsAny<string>()))
            .ReturnsAsync(tag);

        _mapperMock.Setup(x => x.Map<TagResponseModel>(tag))
            .Returns(tagResponseModel);

        // Act
        IActionResult result = await _sut.Create(name);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        TagResponseModel actual = Assert.IsType<TagResponseModel>(createdAtActionResult.Value);
        Assert.Equal(tagResponseModel.Name, actual.Name);
    }


    [Fact]
    public async Task Delete_ReturnsBadRequest_WhenNameIsNull()
    {
        // Arrange
        string name = null!;
        _tagServiceMock.Setup(x => x.DeleteAsync(It.IsAny<string>()))
            .ReturnsAsync(new Result<bool>(new CompanyExistsWithTagNameException(name)));

        // Act
        IActionResult result = await _sut.Delete(name);

        // Assert
        BadRequestObjectResult badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        badRequestResult.Value.Should().BeOfType<CompanyExistsWithTagNameException>();
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
    public async Task AddCompanyTag_ReturnsBadRequest_WhenNameIsNull()
    {
        // Arrange
        int id = 1;
        string name = null!;

        _tagRequestModelValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<TagRequestModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult
            {
                Errors = { new ValidationFailure("Name", "The Name field is required.") },
            });

        // Act
        IActionResult result = await _sut.AddCompanyTag(id, name!);

        // Assert
        BadRequestObjectResult badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
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
        IActionResult result = await _sut.AddCompanyTag(id, name);

        // Assert
        BadRequestObjectResult badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
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
        ValidationResult validationResult = new (new List<ValidationFailure>()
        {
            new ("Name", $"Unable to find company with the id {id}"),
        });

        _tagRequestModelValidatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<TagRequestModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        _companyServiceMock.Setup(x => x.AddTagAsync(It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync(new Result<Company>(new ValidationException(new List<ValidationFailure>())));

        // Act
        IActionResult result = await _sut.AddCompanyTag(id, name);

        // Assert
        BadRequestObjectResult badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        List<string> actual = Assert.IsType<List<string>>(badRequestResult.Value);
        Assert.NotEmpty(actual);
    }

    [Fact]
    public async Task AddCompanyTag_ReturnsUpdatedCompanyResponseModel_WhenTagAddedSuccessfully()
    {
        // Arrange
        int id = 1;
        string name = "tag";
        string companyName = "company";
        ValidationResult validationResult = new ();
        Company companySummaryDto = new (companyName)
            { Id = id, };

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
        IActionResult result = await _sut.AddCompanyTag(id, name);

        // Assert
        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
        CompanyResponseModel actual = Assert.IsType<CompanyResponseModel>(okResult.Value);
        Assert.Equal(companyResponseModel.Id, actual.Id);
    }

    [Fact]
    public async Task DeleteCompanyTag_ReturnsBadRequest_WhenCompanyNotFound()
    {
        // Arrange
        int id = 1;
        string name = "tag";

        _companyServiceMock.Setup(x => x.DeleteTagAsync(It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync(new Result<Company>(new CompanyNotFoundException(id)));

        // Act
        IActionResult result = await _sut.DeleteCompanyTag(id, name);

        // Assert
        BadRequestObjectResult badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.IsType<CompanyNotFoundException>(badRequestResult.Value);
    }

    [Fact]
    public async Task DeleteCompanyTag_ReturnsNoContent_WhenTagDeletedSuccessfully()
    {
        // Arrange
        int id = 1;
        string name = "tag";
        string companyName = "company";
        Company companySummaryDto = new (companyName)
        {
            Id = id,
        };

        _companyServiceMock.Setup(x => x.DeleteTagAsync(It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync(companySummaryDto);

        // Act
        IActionResult result = await _sut.DeleteCompanyTag(id, name);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }
}