using Ardalis.Specification;
using ProjectManagement.CompanyAPI.Domain.Entities;

namespace ProjectManagement.CompanyAPI.Domain.Specifications;

/// <summary>
///     Specification to retrieve all companies that have a given tag name.
/// </summary>
public class AllCompaniesByTagNameSpec : Specification<Company>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AllCompaniesByTagNameSpec" /> class with the given tag name.
    /// </summary>
    /// <param name="tagName">The name of the tag.</param>
    public AllCompaniesByTagNameSpec(string tagName)
    {
        Query
            .Include(x => x.Tags)
            .Where(x => x.Tags.Any(y => y.Name == tagName));
    }
}