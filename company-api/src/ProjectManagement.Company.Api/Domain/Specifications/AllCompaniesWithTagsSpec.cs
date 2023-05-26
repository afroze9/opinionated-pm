using Ardalis.Specification;
using ProjectManagement.CompanyAPI.Domain.Entities;

namespace ProjectManagement.CompanyAPI.Domain.Specifications;

/// <summary>
///     Specification for retrieving all companies with their associated tags.
/// </summary>
public class AllCompaniesWithTagsSpec : Specification<Company>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AllCompaniesWithTagsSpec" /> class.
    /// </summary>
    public AllCompaniesWithTagsSpec()
    {
        Query.Include(x => x.Tags);
    }
}