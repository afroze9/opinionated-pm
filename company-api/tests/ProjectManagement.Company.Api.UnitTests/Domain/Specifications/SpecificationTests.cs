using ProjectManagement.CompanyAPI.Domain.Entities;

namespace ProjectManagement.CompanyAPI.UnitTests.Domain.Specifications;

[ExcludeFromCodeCoverage]
public abstract class SpecificationTests
{
    protected IQueryable<Company> GetCompanies(int count, int tagCount)
    {
        if (count <= 0)
        {
            return new List<Company>()
                .AsQueryable();
        }

        return Enumerable
            .Range(0, count)
            .Select(x => GetCompany(x, tagCount))
            .AsQueryable();
    }

    protected Company GetCompany(int id, int tagCount)
    {
        Company company = new ($"company {id}")
        {
            Id = id,
        };

        company.AddTags(GetTags(tagCount).ToList());
        return company;
    }

    protected IQueryable<Tag> GetTags(int count)
    {
        if (count <= 0)
        {
            return new List<Tag>()
                .AsQueryable();
        }

        return Enumerable
            .Range(0, count)
            .Select(GetTag)
            .AsQueryable();
    }

    protected Tag GetTag(int id)
    {
        return new Tag($"tag {id}")
        {
            Id = id,
        };
    }
}