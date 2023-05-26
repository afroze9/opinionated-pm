using Ardalis.Specification;
using ProjectManagement.CompanyAPI.Domain.Entities;

namespace ProjectManagement.CompanyAPI.Domain.Specifications;

public class TagByNameSpec : Specification<Tag>, ISingleResultSpecification
{
    public TagByNameSpec(string name)
    {
        Query.Where(x => x.Name == name);
    }
}