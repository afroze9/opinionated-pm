using ProjectManagement.CompanyAPI.Domain.Entities;

namespace ProjectManagement.CompanyAPI.UnitTests.Domain.Entities;

[ExcludeFromCodeCoverage]
public class TagTests
{
    [Fact]
    public void Tag_WhenInitialized_CorrectlyInitializes()
    {
        Tag sut = new ("tag a");

        Assert.Equal("tag a", sut.Name);
        Assert.Empty(sut.Companies);
        Assert.Empty(sut.DomainEvents);
        Assert.Equal(string.Empty, sut.CreatedBy);
        Assert.Equal(string.Empty, sut.ModifiedBy);
    }

    [Fact]
    public void Tag_WhenInitializedWithProperties_CorrectlyInitializes()
    {
        DateTime date = DateTime.Now;
        Tag sut = new ("tag a")
        {
            Companies = new List<Company> { new ("company 1") },
            CreatedBy = "cb",
            CreatedOn = date,
            ModifiedBy = "mb",
            ModifiedOn = date,
            Id = 1,
        };

        Assert.Equal(1, sut.Id);
        Assert.Equal("tag a", sut.Name);
        Assert.NotEmpty(sut.Companies);
        Assert.Empty(sut.DomainEvents);
        Assert.Equal("cb", sut.CreatedBy);
        Assert.Equal(date, sut.CreatedOn);
        Assert.Equal("mb", sut.ModifiedBy);
        Assert.Equal(date, sut.ModifiedOn);
    }
}