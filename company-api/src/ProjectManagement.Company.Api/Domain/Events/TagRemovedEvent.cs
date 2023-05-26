using ProjectManagement.CompanyAPI.Contracts;
using ProjectManagement.CompanyAPI.Domain.Entities;

namespace ProjectManagement.CompanyAPI.Domain.Events;

/// <summary>
///     Represents an event that is raised when a tag is removed from a company.
/// </summary>
public class TagRemovedEvent : DomainEventBase
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="TagRemovedEvent" /> class with the specified company and tag.
    /// </summary>
    /// <param name="company">The company that the tag was removed from.</param>
    /// <param name="tag">The tag that was removed from the company.</param>
    public TagRemovedEvent(Company company, Tag tag)
    {
        Company = company;
        Tag = tag;
    }

    /// <summary>
    ///     Gets or sets the company that the tag was removed from.
    /// </summary>
    public Company Company { get; set; }

    /// <summary>
    ///     Gets or sets the tag that was removed from the company.
    /// </summary>
    public Tag Tag { get; set; }
}