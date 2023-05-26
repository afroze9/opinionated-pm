using Ardalis.Specification.EntityFrameworkCore;
using ProjectManagement.CompanyAPI.Domain.Entities;
using ProjectManagement.CompanyAPI.Domain.Specifications;

namespace ProjectManagement.CompanyAPI.UnitTests.Domain.Specifications;

[ExcludeFromCodeCoverage]
public class TagByNameSpecTests : SpecificationTests
{
    [Fact]
    public void TagByNameSpec_WhenUsed_ReturnsCorrectList()
    {
        IQueryable<Tag> tags = GetTags(3);
        TagByNameSpec sut = new ("tag 2");

        SpecificationEvaluator evaluator = new ();
        Tag? result = evaluator.GetQuery(tags, sut).ToList().First();

        Assert.Equal("tag 2", result.Name);
    }
}