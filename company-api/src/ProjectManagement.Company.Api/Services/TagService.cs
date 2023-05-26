using AutoMapper;
using ProjectManagement.CompanyAPI.Abstractions;
using ProjectManagement.CompanyAPI.Domain.Entities;
using ProjectManagement.CompanyAPI.Domain.Specifications;
using ProjectManagement.CompanyAPI.DTO;

namespace ProjectManagement.CompanyAPI.Services;

/// <summary>
///     Service for managing tags.
/// </summary>
public class TagService : ITagService
{
    private readonly IRepository<Company> _companyRepository;
    private readonly IMapper _mapper;
    private readonly IRepository<Tag> _tagRepository;

    /// <summary>
    ///     Initializes a new instance of the <see cref="TagService" /> class.
    /// </summary>
    /// <param name="tagRepository">The tag repository.</param>
    /// <param name="mapper">The mapper.</param>
    /// <param name="companyRepository">The company repository.</param>
    public TagService(IRepository<Tag> tagRepository, IMapper mapper, IRepository<Company> companyRepository)
    {
        _tagRepository = tagRepository;
        _mapper = mapper;
        _companyRepository = companyRepository;
    }

    /// <summary>
    ///     Creates a new tag asynchronously.
    /// </summary>
    /// <param name="name">The name of the tag to create.</param>
    /// <returns>The created tag.</returns>
    public async Task<TagDto> CreateAsync(string name)
    {
        Tag tagToCreate = new (name);
        Tag createdTag = await _tagRepository.AddAsync(tagToCreate);
        return _mapper.Map<TagDto>(createdTag);
    }

    /// <summary>
    ///     Deletes a tag asynchronously.
    /// </summary>
    /// <param name="name">The name of the tag to delete.</param>
    /// <returns>True if the tag was deleted, false otherwise.</returns>
    public async Task<bool> DeleteAsync(string name)
    {
        if (await _companyRepository.AnyAsync(new AllCompaniesByTagNameSpec(name)))
        {
            return false;
        }

        Tag? tagToDelete = await _tagRepository.FirstOrDefaultAsync(new TagByNameSpec(name));

        if (tagToDelete != null)
        {
            await _tagRepository.DeleteAsync(tagToDelete);
        }

        return true;
    }

    /// <summary>
    ///     Gets all tags asynchronously.
    /// </summary>
    /// <returns>A list of all tags.</returns>
    public async Task<List<TagDto>> GetAllAsync()
    {
        List<Tag> tags = await _tagRepository.ListAsync();
        return _mapper.Map<List<TagDto>>(tags);
    }
}