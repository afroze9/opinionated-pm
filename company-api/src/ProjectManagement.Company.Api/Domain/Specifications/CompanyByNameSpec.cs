using Ardalis.Specification;
using ProjectManagement.CompanyAPI.Domain.Entities;

namespace ProjectManagement.CompanyAPI.Domain.Specifications;

/// <summary>
///     Specification for retrieving a company by name.
/// </summary>
public class CompanyByNameSpec : Specification<Company>, ISingleResultSpecification
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CompanyByNameSpec" /> class.
    /// </summary>
    /// <param name="name">The name of the company to retrieve.</param>
    public CompanyByNameSpec(string name)
    {
        Query.Where(x => x.Name == name);
    }
}