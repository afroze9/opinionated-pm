using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Moq;
using ProjectManagement.CompanyAPI.Abstractions;
using ProjectManagement.CompanyAPI.Domain.Entities;
using ProjectManagement.CompanyAPI.Domain.Specifications;
using ProjectManagement.CompanyAPI.DTO;
using ProjectManagement.CompanyAPI.Mapping;
using ProjectManagement.CompanyAPI.Services;

namespace ProjectManagement.CompanyAPI.UnitTests.Services;

[ExcludeFromCodeCoverage]
public class CompanyServiceTests
{
    private readonly Fixture _fixture;
    private readonly IMapper _mapper;

    public CompanyServiceTests()
    {
        _fixture = new Fixture();

        MapperConfiguration mappingConfig = new (mc => { mc.AddProfile(new CompanyProfile()); });
        _mapper = mappingConfig.CreateMapper();
    }

    [Fact]
    public async void GetAllAsync_WhenCalled_ReturnsListOfAllCompanies()
    {
        // Arrange
        List<Company> companies = new ()
            { new (_fixture.Create<string>()), new (_fixture.Create<string>()), new (_fixture.Create<string>()) };

        List<CompanySummaryDto> companySummaries = _mapper.Map<List<CompanySummaryDto>>(companies);

        Mock<IProjectService> projectServiceMock = new ();
        projectServiceMock.Setup(s => s.GetProjectsByCompanyIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new List<ProjectSummaryDto>());

        Mock<IRepository<Company>> companyRepoMock = new ();
        companyRepoMock.Setup(s => s.ListAsync(It.IsAny<AllCompaniesWithTagsSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(companies);

        CompanyService companyService = new (companyRepoMock.Object, null, _mapper, projectServiceMock.Object);

        // Act
        List<CompanySummaryDto> result = await companyService.GetAllAsync();

        // Assert
        result.Should().BeEquivalentTo(companySummaries);
        projectServiceMock.Verify(v => v.GetProjectsByCompanyIdAsync(It.IsAny<int>()),
            Times.Exactly(companies.Count()));

        companyRepoMock.Verify(v => v.ListAsync(It.IsAny<AllCompaniesWithTagsSpec>(), It.IsAny<CancellationToken>()),
            Times.Once());
    }

    [Fact]
    public async void CreateAsync_WhenCalled_ReturnsCreatedCompany()
    {
        // Arrange
        CompanySummaryDto companySummary = _fixture.Create<CompanySummaryDto>();
        Company company = new (companySummary.Name);

        TagDto tagDto = _fixture.Create<TagDto>();
        company.AddTags(new List<Tag>
            { new (tagDto.Name) });

        Tag tag = _mapper.Map<Tag>(tagDto);

        Mock<IRepository<Tag>> tagRepoMock = new ();
        tagRepoMock.Setup(s => s.FirstOrDefaultAsync(It.IsAny<TagByNameSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tag);

        Mock<IRepository<Company>> companyRepoMock = new ();
        companyRepoMock.Setup(s => s.AddAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(company);

        Mock<IProjectService> projectServiceMock = new ();
        CompanyService companyService =
            new (companyRepoMock.Object, tagRepoMock.Object, _mapper, projectServiceMock.Object);

        // Act
        CompanySummaryDto result = await companyService.CreateAsync(companySummary);

        // Assert
        result.Should().BeEquivalentTo(_mapper.Map<CompanySummaryDto>(company));
        companyRepoMock.Verify(v => v.AddAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async void CreateAsync_WhenDbTagDoesNotExist_ReturnsCreatedCompanyWithTag()
    {
        // Arrange
        CompanySummaryDto companySummary = _fixture.Create<CompanySummaryDto>();
        Company company = new (companySummary.Name);

        TagDto tagDto = _fixture.Create<TagDto>();
        company.AddTags(new List<Tag>
            { new (tagDto.Name) });

        Tag tag = _mapper.Map<Tag>(tagDto);

        Mock<IRepository<Tag>> tagRepoMock = new ();
        Mock<IRepository<Company>> companyRepoMock = new ();
        companyRepoMock.Setup(s => s.AddAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(company);

        Mock<IProjectService> projectServiceMock = new ();
        CompanyService companyService =
            new (companyRepoMock.Object, tagRepoMock.Object, _mapper, projectServiceMock.Object);

        // Act
        CompanySummaryDto result = await companyService.CreateAsync(companySummary);

        // Assert
        result.Should().BeEquivalentTo(_mapper.Map<CompanySummaryDto>(company));
        companyRepoMock.Verify(v => v.AddAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async void GetByIdAsync_WhenCalled_ReturnsCompanyWithGivenId()
    {
        // Arrange
        int id = _fixture.Create<int>();
        string companyName = _fixture.Create<string>();
        Company company = new (companyName);
        company.Id = id;

        Mock<IProjectService> projectServiceMock = new ();
        projectServiceMock.Setup(s => s.GetProjectsByCompanyIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new List<ProjectSummaryDto>());

        Mock<IRepository<Company>> companyRepoMock = new ();
        companyRepoMock.Setup(s =>
                s.FirstOrDefaultAsync(It.IsAny<CompanyByIdWithTagsSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(company);

        CompanyService companyService = new (companyRepoMock.Object, null, _mapper, projectServiceMock.Object);

        // Act
        CompanyDto? result = await companyService.GetByIdAsync(id);

        // Assert
        result.Should().BeEquivalentTo(_mapper.Map<CompanyDto>(company));
        companyRepoMock.Verify(
            v => v.FirstOrDefaultAsync(It.IsAny<CompanyByIdWithTagsSpec>(), It.IsAny<CancellationToken>()),
            Times.Once());

        projectServiceMock.Verify(v => v.GetProjectsByCompanyIdAsync(It.IsAny<int>()), Times.Once());
    }

    [Fact]
    public async void GetByIdAsync_WhenCompanyNotInDb_ReturnsNull()
    {
        // Arrange
        int id = _fixture.Create<int>();

        Mock<IProjectService> projectServiceMock = new ();
        Mock<IRepository<Company>> companyRepoMock = new ();
        CompanyService companyService = new (companyRepoMock.Object, null, _mapper, projectServiceMock.Object);

        // Act
        CompanyDto? result = await companyService.GetByIdAsync(id);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async void UpdateNameAsync_WhenCalledWithValidIdAndNewName_ReturnsUpdatedCompanySummary()
    {
        // Arrange
        int id = _fixture.Create<int>();
        string name = _fixture.Create<string>();

        Company company = new (name);
        company.Id = id;

        Mock<IRepository<Company>> companyRepoMock = new ();
        companyRepoMock.Setup(s => s.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(company);

        CompanyService companyService = new (companyRepoMock.Object, null, _mapper, null);

        // Act
        CompanySummaryDto? result = await companyService.UpdateNameAsync(id, name);

        // Assert
        result.Should().BeEquivalentTo(_mapper.Map<CompanySummaryDto>(company));
        companyRepoMock.Verify(v => v.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once());
        companyRepoMock.Verify(v => v.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddTagAsync_WhenCalled_ReturnsUpdatedCompany()
    {
        // Arrange
        int id = _fixture.Create<int>();
        string tagName = _fixture.Create<string>();
        string companyName = _fixture.Create<string>();
        TagDto tagDto = new ()
            { Name = tagName };

        Tag existingTag = new (tagName);

        Company company = new (companyName);
        company.AddTag(existingTag);

        Mock<IRepository<Tag>> tagRepoMock = new ();
        tagRepoMock.Setup(s => s.FirstOrDefaultAsync(It.IsAny<TagByNameSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTag);

        Mock<IRepository<Company>> companyRepoMock = new ();
        companyRepoMock.Setup(s =>
                s.FirstOrDefaultAsync(It.IsAny<CompanyByIdWithTagsSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(company);

        Mock<IProjectService> projectServiceMock = new ();
        CompanyService companyService =
            new (companyRepoMock.Object, tagRepoMock.Object, _mapper, projectServiceMock.Object);

        // Act
        CompanySummaryDto? result = await companyService.AddTagAsync(id, tagName);

        // Assert
        result.Should().BeEquivalentTo(_mapper.Map<CompanySummaryDto>(company));
        tagRepoMock.Verify(v => v.FirstOrDefaultAsync(It.IsAny<TagByNameSpec>(), It.IsAny<CancellationToken>()),
            Times.Once());

        companyRepoMock.Verify(
            v => v.FirstOrDefaultAsync(It.IsAny<CompanyByIdWithTagsSpec>(), It.IsAny<CancellationToken>()),
            Times.Once());

        companyRepoMock.Verify(v => v.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async void DeleteTagAsync_WhenCalled_ReturnsUpdatedCompany()
    {
        // Arrange
        int id = _fixture.Create<int>();
        string tagName = _fixture.Create<string>();
        string companyName = _fixture.Create<string>();

        // Use the existing tag's constructor to create a new Tag object with the same name.
        Tag newTag = new (tagName);
        Company company = new (companyName);
        company.Id = id;
        company.AddTag(newTag);

        Mock<IRepository<Company>> companyRepoMock = new ();
        companyRepoMock.Setup(s =>
                s.FirstOrDefaultAsync(It.IsAny<CompanyByIdWithTagsSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(company);

        Mock<IProjectService> projectServiceMock = new ();
        CompanyService companyService = new (companyRepoMock.Object, null, _mapper, projectServiceMock.Object);

        // Act
        CompanySummaryDto? result = await companyService.DeleteTagAsync(id, tagName);

        // Assert
        result.Should().BeEquivalentTo(_mapper.Map<CompanySummaryDto>(company));
        companyRepoMock.Verify(
            v => v.FirstOrDefaultAsync(It.IsAny<CompanyByIdWithTagsSpec>(), It.IsAny<CancellationToken>()),
            Times.Once());

        companyRepoMock.Verify(v => v.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
    }
}