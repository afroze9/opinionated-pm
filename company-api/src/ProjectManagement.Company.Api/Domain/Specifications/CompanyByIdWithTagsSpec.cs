using Ardalis.Specification;
using ProjectManagement.CompanyAPI.Domain.Entities;

namespace ProjectManagement.CompanyAPI.Domain.Specifications;

/// <summary>
///     Specification for retrieving a company by ID with its associated tags.
/// </summary>
public class CompanyByIdWithTagsSpec : Specification<Company>, ISingleResultSpecification
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CompanyByIdWithTagsSpec" /> class.
    /// </summary>
    /// <param name="id">The ID of the company to retrieve.</param>
    public CompanyByIdWithTagsSpec(int id)
    {
        Query
            .Where(x => x.Id == id)
            .Include(x => x.Tags);
    }
}