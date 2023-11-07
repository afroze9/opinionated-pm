using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using LanguageExt.Common;
using Microsoft.AspNetCore.Mvc;
using Nexus.CompanyAPI.Abstractions;
using Nexus.CompanyAPI.Controllers;
using Nexus.CompanyAPI.Entities;
using Nexus.CompanyAPI.Exceptions;
using Nexus.CompanyAPI.Model;
using NSubstitute;

namespace Nexus.CompanyAPI.UnitTests.Controllers;

[ExcludeFromCodeCoverage]
public class TagControllerTests
{
    private readonly ICompanyService _companyServiceMock = Substitute.For<ICompanyService>();
    private readonly IMapper _mapperMock = Substitute.For<IMapper>();

    private readonly TagController _sut;
    private readonly IValidator<TagRequestModel> _tagRequestModelValidatorMock = Substitute.For<IValidator<TagRequestModel>>();
    private readonly ITagService _tagServiceMock = Substitute.For<ITagService>();

    public TagControllerTests()
    {
        _sut = new TagController(
            _mapperMock, 
            _companyServiceMock,
            _tagServiceMock,
            _tagRequestModelValidatorMock);
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

        _tagServiceMock.GetAllAsync()
            .Returns(tags);

        _mapperMock.Map<List<TagResponseModel>>(tags)
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

        _tagServiceMock.GetAllAsync()
            .Returns(tags);

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
            .ValidateAsync(Arg.Any<TagRequestModel>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult
            {
                Errors = { new ValidationFailure("Name", "The Name field is required.") },
            });

        _tagServiceMock.CreateAsync(Arg.Any<string>())
            .Returns(new Result<Tag>(new ValidationException(new List<ValidationFailure>()
            {
                new ("Name", "The Name field is required."),
            })));

        // Act
        IActionResult result = await _sut.Create(name);

        // Assert
        BadRequestObjectResult badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        ValidationException? actual = Assert.IsType<ValidationException>(badRequestResult.Value);
        Assert.Single(actual.Errors);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenValidationFails()
    {
        // Arrange
        string name = "tag";
        ValidationResult validationResult = new (new[] { new ValidationFailure("Name", "error") });

        _tagRequestModelValidatorMock
            .ValidateAsync(Arg.Any<TagRequestModel>(), Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _tagServiceMock.CreateAsync(Arg.Any<string>())
            .Returns(new Result<Tag>(new ValidationException(new List<ValidationFailure>()
            {
                new ("Name", "The Name field is required."),
            })));

        // Act
        IActionResult result = await _sut.Create(name);

        // Assert
        BadRequestObjectResult badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        ValidationException? actual = Assert.IsType<ValidationException>(badRequestResult.Value);
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
            .ValidateAsync(Arg.Any<TagRequestModel>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _tagServiceMock.CreateAsync(Arg.Any<string>())
            .Returns(tag);

        _mapperMock.Map<TagResponseModel>(tag)
            .Returns(tagResponseModel);

        // Act
        IActionResult result = await _sut.Create(name);

        // Assert
        CreatedAtActionResult? createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        TagResponseModel actual = Assert.IsType<TagResponseModel>(createdAtActionResult.Value);
        Assert.Equal(tagResponseModel.Name, actual.Name);
    }


    [Fact]
    public async Task Delete_ReturnsBadRequest_WhenNameIsNull()
    {
        // Arrange
        string name = null!;
        _tagServiceMock.DeleteAsync(Arg.Any<string>())
            .Returns(new Result<bool>(new CompanyExistsWithTagNameException(name)));

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
        _tagServiceMock.DeleteAsync(Arg.Any<string>())
            .Returns(true);

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
            .ValidateAsync(Arg.Any<TagRequestModel>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult
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
            .ValidateAsync(Arg.Any<TagRequestModel>(), Arg.Any<CancellationToken>())
            .Returns(validationResult);

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
            .ValidateAsync(Arg.Any<TagRequestModel>(), Arg.Any<CancellationToken>())
            .Returns(validationResult);

        _companyServiceMock.AddTagAsync(Arg.Any<int>(), Arg.Any<string>())
            .Returns(new Result<Company>(new ValidationException(new List<ValidationFailure>())));

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
            .ValidateAsync(Arg.Any<TagRequestModel>(), Arg.Any<CancellationToken>())
            .Returns(validationResult);

        _companyServiceMock.AddTagAsync(Arg.Any<int>(), Arg.Any<string>())
            .Returns(companySummaryDto);

        _mapperMock.Map<CompanyResponseModel>(companySummaryDto)
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

        _companyServiceMock.DeleteTagAsync(Arg.Any<int>(), Arg.Any<string>())
            .Returns(new Result<Company>(new CompanyNotFoundException(id)));

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

        _companyServiceMock.DeleteTagAsync(Arg.Any<int>(), Arg.Any<string>())
            .Returns(companySummaryDto);

        // Act
        IActionResult result = await _sut.DeleteCompanyTag(id, name);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }
}