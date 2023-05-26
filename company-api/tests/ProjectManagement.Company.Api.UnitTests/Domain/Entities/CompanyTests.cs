using ProjectManagement.CompanyAPI.Contracts;
using ProjectManagement.CompanyAPI.Domain.Entities;
using ProjectManagement.CompanyAPI.Domain.Events;

namespace ProjectManagement.CompanyAPI.UnitTests.Domain.Entities;

[ExcludeFromCodeCoverage]
public class CompanyTests
{
    [Fact]
    public void Company_WhenInitialized_CorrectlyInitializes()
    {
        Company sut = new ("company a");

        Assert.Equal("company a", sut.Name);
        Assert.Empty(sut.Tags);
        Assert.Empty(sut.DomainEvents);
        Assert.Equal(string.Empty, sut.CreatedBy);
        Assert.Equal(string.Empty, sut.ModifiedBy);
    }

    [Fact]
    public void Company_WhenInitializedWithProperties_CorrectlyInitializes()
    {
        DateTime date = DateTime.Now;
        Company sut = new ("company a")
        {
            Tags = new List<Tag> { new ("tag 1") },
            CreatedBy = "cb",
            CreatedOn = date,
            ModifiedBy = "mb",
            ModifiedOn = date,
            Id = 1,
        };

        Assert.Equal(1, sut.Id);
        Assert.Equal("company a", sut.Name);
        Assert.NotEmpty(sut.Tags);
        Assert.Empty(sut.DomainEvents);
        Assert.Equal("cb", sut.CreatedBy);
        Assert.Equal(date, sut.CreatedOn);
        Assert.Equal("mb", sut.ModifiedBy);
        Assert.Equal(date, sut.ModifiedOn);
    }

    [Fact]
    public void Company_NameUpdated_CorrectlyUpdatesName()
    {
        Company sut = new ("company a");

        sut.UpdateName("updated a");

        Assert.Equal("updated a", sut.Name);
    }

    [Fact]
    public void Company_WhenTagAdded_SucessfullyAddsTag()
    {
        Company sut = new ("company a");
        Tag[] tags = { new ("tag 1"), new ("tag 2") };

        sut.AddTags(tags.ToList());

        Assert.NotEmpty(sut.Tags);
        Assert.Equal(2, sut.Tags.Count);
        Assert.Equal("tag 1", sut.Tags.First().Name);
        Assert.Equal(typeof(NewTagAddedEvent), sut.DomainEvents.First().GetType());
    }

    [Fact]
    public void Company_WhenTagRemoved_SucessfullyRemovesTag()
    {
        Company sut = new ("company a");
        Tag[] tags = { new ("tag 1"), new ("tag 2") };

        sut.AddTags(tags.ToList());
        sut.RemoveTag("tag 1");

        Assert.NotEmpty(sut.Tags);
        Assert.Single(sut.Tags);
        Assert.Equal("tag 2", sut.Tags.First().Name);
        Assert.Equal(typeof(TagRemovedEvent), sut.DomainEvents.Last().GetType());
    }

    [Fact]
    public void Company_WhenTagsRemoved_SucessfullyRemovesTags()
    {
        Company sut = new ("company a");
        Tag[] tags = { new ("tag 1"), new ("tag 2") };

        sut.AddTags(tags.ToList());
        sut.RemoveTags();

        Assert.Empty(sut.Tags);
        List<DomainEventBase> removalEvents =
            sut.DomainEvents.Where(x => x.GetType() == typeof(TagRemovedEvent)).ToList();

        Assert.NotEmpty(removalEvents);
        Assert.Equal(2, removalEvents.Count);
    }

    [Fact]
    public void CompanyWithNoTags_WhenTagsRemoved_SucessfullyRemovesTags()
    {
        Company sut = new ("company a");
        sut.RemoveTags();

        Assert.Empty(sut.Tags);
        Assert.Empty(sut.DomainEvents);
    }

    [Fact]
    public void Company_ClearDomainEvents_ClearsEvents()
    {
        Company sut = new ("company a");
        Tag[] tags = { new ("tag 1"), new ("tag 2") };

        sut.AddTags(tags.ToList());
        sut.RemoveTags();
        sut.ClearDomainEvents();

        Assert.Empty(sut.DomainEvents);
    }
}