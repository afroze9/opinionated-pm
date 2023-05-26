using Ardalis.Specification.EntityFrameworkCore;
using ProjectManagement.CompanyAPI.Domain.Entities;
using ProjectManagement.CompanyAPI.Domain.Specifications;

namespace ProjectManagement.CompanyAPI.UnitTests.Domain.Specifications;

[ExcludeFromCodeCoverage]
public class CompanyByIdWithTagsSpecTests : SpecificationTests
{
    [Fact]
    public void CompanyByIdWithTagsSpec_WhenUsed_ReturnsCorrectList()
    {
        IQueryable<Company>? companies = GetCompanies(3, 1);
        CompanyByIdWithTagsSpec? sut = new (2);

        SpecificationEvaluator evaluator = new ();
        Company? result = evaluator.GetQuery(companies, sut).ToList().First();

        Assert.Equal(2, result.Id);
    }
}