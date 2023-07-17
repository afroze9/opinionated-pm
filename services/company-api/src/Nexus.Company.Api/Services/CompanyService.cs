using AutoMapper;
using FluentValidation;
using LanguageExt.Common;
using Nexus.CompanyAPI.Abstractions;
using Nexus.CompanyAPI.Data;
using Nexus.CompanyAPI.DTO;
using Nexus.CompanyAPI.Entities;
using Nexus.CompanyAPI.Exceptions;
using ValidationException = FluentValidation.ValidationException;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace Nexus.CompanyAPI.Services;

/// <summary>
///     Service for managing companies and their associated tags and projects.
/// </summary>
public class CompanyService : ICompanyService
{
    private readonly IMapper _mapper;
    private readonly IProjectService _projectService;
    private readonly UnitOfWork _unitOfWork;
    private readonly ILogger<CompanyService> _logger;
    private readonly IValidator<Company> _companyValidator;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CompanyService" /> class.
    /// </summary>
    /// <param name="mapper">The mapper.</param>
    /// <param name="projectService">The project service.</param>
    /// <param name="unitOfWork">Unit of work for the project.</param>
    /// <param name="logger">Logger for the service.</param>
    /// <param name="companyValidator">Validator for the company entity.</param>
    public CompanyService(
        IMapper mapper,
        IProjectService projectService,
        UnitOfWork unitOfWork, 
        ILogger<CompanyService> logger, 
        IValidator<Company> companyValidator)
    {
        _mapper = mapper;
        _projectService = projectService;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _companyValidator = companyValidator;
    }

    /// <summary>
    ///     Gets all companies asynchronously.
    /// </summary>
    /// <returns>A list of all companies.</returns>
    public async Task<List<CompanySummaryDto>> GetAllAsync()
    {
        List<Company> companies = await _unitOfWork.Companies.AllCompaniesWithTagsAsync();
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
    /// <param name="company">The company entity to create.</param>
    /// <returns>The created company.</returns>
    public async Task<Result<Company>> CreateAsync(Company company)
    {
        ValidationResult validationResult = await _companyValidator.ValidateAsync(company);

        if (!validationResult.IsValid)
        {
            return new Result<Company>(new ValidationException(validationResult.Errors));
        }

        try
        {
            _unitOfWork.BeginTransaction();
            List<Tag> tagsToAdd = new ();
            
            if (company.Tags.Count != 0)
            {
                foreach (string tagName in company.Tags.Select(t => t.Name))
                {
                    Tag? dbTag = await _unitOfWork.Tags.GetByNameAsync(tagName);

                    if (dbTag != null)
                    {
                        tagsToAdd.Add(dbTag);
                    }
                    else
                    {
                        Tag tagToAdd = new (tagName);
                        _unitOfWork.Tags.Add(tagToAdd);
                        tagsToAdd.Add(tagToAdd);
                    }
                }
            }

            company.AddTags(tagsToAdd);
            _unitOfWork.Companies.Add(company);
            _unitOfWork.Commit();

            return company;
        }
        catch (Exception ex)
        {
            CreateCompanyException companyException = new (ex);
            
            _logger.LogInformation(EventIds.CreateCompanyTransactionError, companyException, CreateCompanyException.ExceptionMessage);
            _unitOfWork.Rollback();
            
            return new Result<Company>(companyException);
        }
    }

    /// <summary>
    ///     Gets a company by ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the company to get.</param>
    /// <returns>The company with the specified ID, or null if not found.</returns>
    public async Task<Result<CompanyDto>> GetByIdAsync(int id)
    {
        Company? company = await _unitOfWork.Companies.GetByIdWithTagsAsync(id);
        
        if (company == null)
        {
            return new Result<CompanyDto>(new CompanyNotFoundException(id));
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
    public async Task<Result<Company>> UpdateNameAsync(int id, string name)
    {
        Company? companyToUpdate = await _unitOfWork.Companies.GetByIdAsync(id);
        
        if (companyToUpdate == null)
        {
            return new Result<Company>(new CompanyNotFoundException(id));
        }

        if (await _unitOfWork.Companies.AnyOtherCompaniesWithSameNameAsync(id, name))
        {
            return new Result<Company>(new AnotherCompanyExistsWithSameNameException(name));
        }
        
        companyToUpdate.UpdateName(name);
        ValidationResult? validationResult = await _companyValidator.ValidateAsync(companyToUpdate);
        if (!validationResult.IsValid)
        {
            return new Result<Company>(new ValidationException(validationResult.Errors));
        }
        
        _unitOfWork.BeginTransaction();
        _unitOfWork.Commit();

        return companyToUpdate;
    }

    /// <summary>
    ///     Adds a tag to a company asynchronously.
    /// </summary>
    /// <param name="id">The ID of the company to add the tag to.</param>
    /// <param name="tagName">The name of the tag to add.</param>
    /// <returns>The updated company, or null if not found.</returns>
    public async Task<Result<Company>> AddTagAsync(int id, string tagName)
    {
        Company? companyToUpdate = await _unitOfWork.Companies.GetByIdAsync(id);
        
        if (companyToUpdate == null)
        {
            return new Result<Company>(new CompanyNotFoundException(id));
        }

        Tag? dbTag = await _unitOfWork.Tags.GetByNameAsync(tagName);
        
        if (dbTag != null)
        {
            companyToUpdate.AddTag(dbTag);
        }
        else
        {
            Tag tagToAdd = new (tagName);
            companyToUpdate.AddTag(tagToAdd);
        }
        
        _unitOfWork.BeginTransaction();
        _unitOfWork.Commit();

        return companyToUpdate;
    }

    /// <summary>
    ///     Deletes a tag from a company asynchronously.
    /// </summary>
    /// <param name="id">The ID of the company to delete the tag from.</param>
    /// <param name="tagName">The name of the tag to delete.</param>
    /// <returns>The updated company, or null if not found.</returns>
    public async Task<Result<Company>> DeleteTagAsync(int id, string tagName)
    {
        Company? companyToUpdate = await _unitOfWork.Companies.GetByIdAsync(id);
        
        if (companyToUpdate == null)
        {
            return new Result<Company>(new CompanyNotFoundException(id));
        }
        
        companyToUpdate.RemoveTag(tagName);
        _unitOfWork.BeginTransaction();
        _unitOfWork.Commit();
        
        return companyToUpdate;
    }

    /// <summary>
    ///     Deletes a company asynchronously.
    /// </summary>
    /// <param name="id">The ID of the company to delete.</param>
    public async Task<Result<bool>> DeleteAsync(int id)
    {

        Company? companyToDelete = await _unitOfWork.Companies.GetByIdAsync(id);
        
        if (companyToDelete == null)
        {
            return true;
        }
        
        companyToDelete.RemoveTags();
        _unitOfWork.Companies.Delete(companyToDelete);
        
        _unitOfWork.BeginTransaction();
        _unitOfWork.Commit();

        return true;
    }
}