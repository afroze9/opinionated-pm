using ProjectManagement.CompanyAPI.Contracts;
using ProjectManagement.CompanyAPI.Domain.Entities;

namespace ProjectManagement.CompanyAPI.Domain.Events;

/// <summary>
///     Represents an event that is raised when a new tag is added to a company.
/// </summary>
public class NewTagAddedEvent : DomainEventBase
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="NewTagAddedEvent" /> class with the specified company and tag.
    /// </summary>
    /// <param name="company">The company that the tag was added to.</param>
    /// <param name="tag">The tag that was added to the company.</param>
    public NewTagAddedEvent(Company company, Tag tag)
    {
        Company = company;
        Tag = tag;
    }

    /// <summary>
    ///     Gets or sets the company that the tag was added to.
    /// </summary>
    public Company Company { get; set; }

    /// <summary>
    ///     Gets or sets the tag that was added to the company.
    /// </summary>
    public Tag Tag { get; set; }
}