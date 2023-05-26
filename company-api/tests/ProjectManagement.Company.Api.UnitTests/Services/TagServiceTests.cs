using AutoMapper;
using Moq;
using ProjectManagement.CompanyAPI.Abstractions;
using ProjectManagement.CompanyAPI.Domain.Entities;
using ProjectManagement.CompanyAPI.Domain.Specifications;
using ProjectManagement.CompanyAPI.DTO;
using ProjectManagement.CompanyAPI.Services;

namespace ProjectManagement.CompanyAPI.UnitTests.Services;

[ExcludeFromCodeCoverage]
public class TagServiceTests
{
    private readonly Mock<IRepository<Company>> _companyRepositoryMock = new ();
    private readonly Mock<IMapper> _mapperMock = new ();
    private readonly Mock<IRepository<Tag>> _tagRepositoryMock = new ();

    private readonly ITagService _tagService;

    public TagServiceTests()
    {
        _tagService = new TagService(_tagRepositoryMock.Object, _mapperMock.Object, _companyRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateNewTag_WhenValidNameIsProvided()
    {
        // Arrange
        string tagName = "test tag";
        Tag createdTag = new (tagName);
        TagDto expectedTagDto = new ()
            { Name = tagName };

        _mapperMock.Setup(x => x.Map<TagDto>(It.IsAny<Tag>())).Returns(expectedTagDto);
        _tagRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Tag>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdTag);

        // Act
        TagDto result = await _tagService.CreateAsync(tagName);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedTagDto.Name, result.Name);
        _tagRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Tag>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenAnyCompanyHasTagName()
    {
        // Arrange
        string tagName = "test tag";
        _companyRepositoryMock
            .Setup(x => x.AnyAsync(It.IsAny<AllCompaniesByTagNameSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        bool result = await _tagService.DeleteAsync(tagName);

        // Assert
        Assert.False(result);
        _companyRepositoryMock.Verify(
            x => x.AnyAsync(It.IsAny<AllCompaniesByTagNameSpec>(), It.IsAny<CancellationToken>()), Times.Once);

        _tagRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<Tag>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteTag_WhenTagExists()
    {
        // Arrange
        string tagName = "test tag";
        Tag tagToDelete = new (tagName);

        _companyRepositoryMock
            .Setup(x => x.AnyAsync(It.IsAny<AllCompaniesByTagNameSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _tagRepositoryMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<TagByNameSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tagToDelete);

        // Act
        bool result = await _tagService.DeleteAsync(tagName);

        // Assert
        Assert.True(result);
        _tagRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<Tag>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_WhenTagDoesNotExist()
    {
        // Arrange
        string tagName = "test tag";
        Tag? tagToDelete = null;

        _companyRepositoryMock
            .Setup(x => x.AnyAsync(It.IsAny<AllCompaniesByTagNameSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _tagRepositoryMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<TagByNameSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tagToDelete);

        // Act
        bool result = await _tagService.DeleteAsync(tagName);

        // Assert
        Assert.True(result);
        _tagRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<Tag>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllTags()
    {
        // Arrange
        List<Tag> tags = new List<Tag> { new ("test tag 1"), new ("test tag 2") };
        List<TagDto> expectedTagDtos = new List<TagDto>
        {
            new ()
                { Name = tags[0].Name },
            new ()
                { Name = tags[1].Name },
        };

        _mapperMock.Setup(x => x.Map<List<TagDto>>(It.IsAny<List<Tag>>())).Returns(expectedTagDtos);
        _tagRepositoryMock.Setup(x => x.ListAsync(It.IsAny<CancellationToken>())).ReturnsAsync(tags);

        // Act
        List<TagDto> result = await _tagService.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedTagDtos.Count, result.Count);

        for (int i = 0; i < expectedTagDtos.Count; i++)
        {
            Assert.Equal(expectedTagDtos[i].Name, result[i].Name);
        }

        _tagRepositoryMock.Verify(x => x.ListAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}