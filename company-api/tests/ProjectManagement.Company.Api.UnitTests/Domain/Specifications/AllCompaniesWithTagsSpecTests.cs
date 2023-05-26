using Ardalis.Specification.EntityFrameworkCore;
using ProjectManagement.CompanyAPI.Domain.Entities;
using ProjectManagement.CompanyAPI.Domain.Specifications;

namespace ProjectManagement.CompanyAPI.UnitTests.Domain.Specifications;

[ExcludeFromCodeCoverage]
public class AllCompaniesWithTagsSpecTests : SpecificationTests
{
    [Fact]
    public void AllCompaniesWithTagsSpec_WhenUsed_ReturnsCorrectList()
    {
        IQueryable<Company>? companies = GetCompanies(3, 1);
        AllCompaniesWithTagsSpec? sut = new ();

        SpecificationEvaluator evaluator = new ();
        List<Company>? result = evaluator.GetQuery(companies, sut).ToList();

        Assert.Equal(3, result.Count);
    }
}