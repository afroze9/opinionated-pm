using AutoMapper;
using ProjectManagement.CompanyAPI.Abstractions;
using ProjectManagement.CompanyAPI.Domain.Entities;
using ProjectManagement.CompanyAPI.Domain.Specifications;
using ProjectManagement.CompanyAPI.DTO;
using ProjectManagement.Persistence.Abstractions;

namespace ProjectManagement.CompanyAPI.Services;

/// <summary>
///     Service for managing companies and their associated tags and projects.
/// </summary>
public class CompanyService : ICompanyService
{
    private readonly IRepository<Company> _companyRepository;
    private readonly IMapper _mapper;
    private readonly IProjectService _projectService;
    private readonly IRepository<Tag> _tagRepository;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CompanyService" /> class.
    /// </summary>
    /// <param name="companyRepository">The company repository.</param>
    /// <param name="tagRepository">The tag repository.</param>
    /// <param name="mapper">The mapper.</param>
    /// <param name="projectService">The project service.</param>
    public CompanyService(IRepository<Company> companyRepository, IRepository<Tag> tagRepository, IMapper mapper,
        IProjectService projectService)
    {
        _companyRepository = companyRepository;
        _tagRepository = tagRepository;
        _mapper = mapper;
        _projectService = projectService;
    }

    /// <summary>
    ///     Gets all companies asynchronously.
    /// </summary>
    /// <returns>A list of all companies.</returns>
    public async Task<List<CompanySummaryDto>> GetAllAsync()
    {
        List<Company> companies = await _companyRepository.ListAsync(new AllCompaniesWithTagsSpec());
        List<CompanySummaryDto> mappedCompanies = _mapper.Map<List<CompanySummaryDto>>(companies);

        foreach (CompanySummaryDto company in mappedCompanies)
        {
            List<ProjectSummaryDto> projects = await _projectService.GetProjectsByCompanyIdAsync(company.Id);
            company.ProjectCount = projects.Count;
        }
        
        return mappedCompanies;
    }

    /// <summary>
    ///     Creates a new company asynchronously.
    /// </summary>
    /// <param name="companySummary">The company summary.</param>
    /// <returns>The created company.</returns>
    public async Task<CompanySummaryDto> CreateAsync(CompanySummaryDto companySummary)
    {
        Company companyToCreate = new (companySummary.Name);
        List<Tag> tagsToAdd = new ();

        if (companySummary.Tags.Count != 0)
        {
            foreach (string tagName in companySummary.Tags.Select(t => t.Name))
            {
                Tag? dbTag = await _tagRepository.FirstOrDefaultAsync(new TagByNameSpec(tagName));

                if (dbTag != null)
                {
                    tagsToAdd.Add(dbTag);
                }
                else
                {
                    Tag addedTag = await _tagRepository.AddAsync(new Tag(tagName));
                    tagsToAdd.Add(addedTag);
                }
            }
        }

        companyToCreate.AddTags(tagsToAdd);
        Company createdCompany = await _companyRepository.AddAsync(companyToCreate);

        return _mapper.Map<CompanySummaryDto>(createdCompany);
    }

    /// <summary>
    ///     Gets a company by ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the company to get.</param>
    /// <returns>The company with the specified ID, or null if not found.</returns>
    public async Task<CompanyDto?> GetByIdAsync(int id)
    {
        Company? company = await _companyRepository.FirstOrDefaultAsync(new CompanyByIdWithTagsSpec(id));

        if (company == null)
        {
            return null;
        }

        CompanyDto mappedCompanySummary = _mapper.Map<CompanyDto>(company);
        List<ProjectSummaryDto> projects = await _projectService.GetProjectsByCompanyIdAsync(id);
        mappedCompanySummary.Projects = projects;

        return mappedCompanySummary;
    }

    /// <summary>
    ///     Updates the name of a company asynchronously.
    /// </summary>
    /// <param name="id">The ID of the company to update.</param>
    /// <param name="name">The new name of the company.</param>
    /// <returns>The updated company, or null if not found.</returns>
    public async Task<CompanySummaryDto?> UpdateNameAsync(int id, string name)
    {
        Company? companyToUpdate = await _companyRepository.GetByIdAsync(id);

        if (companyToUpdate == null)
        {
            return null;
        }

        companyToUpdate.UpdateName(name);
        await _companyRepository.SaveChangesAsync();

        CompanySummaryDto summaryDto = _mapper.Map<CompanySummaryDto>(companyToUpdate);
        return summaryDto;
    }

    /// <summary>
    ///     Adds a tag to a company asynchronously.
    /// </summary>
    /// <param name="id">The ID of the company to add the tag to.</param>
    /// <param name="tagName">The name of the tag to add.</param>
    /// <returns>The updated company, or null if not found.</returns>
    public async Task<CompanySummaryDto?> AddTagAsync(int id, string tagName)
    {
        Company? companyToUpdate = await _companyRepository.FirstOrDefaultAsync(new CompanyByIdWithTagsSpec(id));

        if (companyToUpdate == null)
        {
            return null;
        }

        Tag? dbTag = await _tagRepository.FirstOrDefaultAsync(new TagByNameSpec(tagName));

        if (dbTag != null)
        {
            companyToUpdate.AddTag(dbTag);
        }
        else
        {
            Tag addedTag = await _tagRepository.AddAsync(new Tag(tagName));
            companyToUpdate.AddTag(addedTag);
        }

        await _companyRepository.SaveChangesAsync();
        return _mapper.Map<CompanySummaryDto>(companyToUpdate);
    }

    /// <summary>
    ///     Deletes a tag from a company asynchronously.
    /// </summary>
    /// <param name="id">The ID of the company to delete the tag from.</param>
    /// <param name="tagName">The name of the tag to delete.</param>
    /// <returns>The updated company, or null if not found.</returns>
    public async Task<CompanySummaryDto?> DeleteTagAsync(int id, string tagName)
    {
        Company? companyToUpdate = await _companyRepository.FirstOrDefaultAsync(new CompanyByIdWithTagsSpec(id));

        if (companyToUpdate == null)
        {
            return null;
        }

        companyToUpdate.RemoveTag(tagName);

        await _companyRepository.SaveChangesAsync();
        CompanySummaryDto summaryDto = _mapper.Map<CompanySummaryDto>(companyToUpdate);

        return summaryDto;
    }

    /// <summary>
    ///     Deletes a company asynchronously.
    /// </summary>
    /// <param name="id">The ID of the company to delete.</param>
    public async Task DeleteAsync(int id)
    {
        Company? companyToDelete = await _companyRepository.FirstOrDefaultAsync(new CompanyByIdWithTagsSpec(id));

        if (companyToDelete == null)
        {
            return;
        }

        companyToDelete.RemoveTags();

        await _companyRepository.SaveChangesAsync();
        await _companyRepository.DeleteAsync(companyToDelete);
    }
}