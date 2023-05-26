using ProjectManagement.CompanyAPI.Abstractions;
using ProjectManagement.CompanyAPI.Contracts;

namespace ProjectManagement.CompanyAPI.Domain.Entities;

/// <summary>
///     Represents a tag that can be associated with one or more companies.
/// </summary>
public class Tag : EntityBase, IAggregateRoot, IAuditable<string>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Tag" /> class with the specified name.
    /// </summary>
    /// <param name="name">The name of the tag.</param>
    public Tag(string name)
    {
        Name = name;
    }

    /// <summary>
    ///     Gets the name of the tag.
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     Gets or sets the list of companies that are associated with this tag.
    /// </summary>
    public virtual List<Company> Companies { get; set; } = new ();

    /// <summary>
    ///     Gets or sets the name of the user who created this entity.
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the date and time when this entity was created.
    /// </summary>
    public DateTime CreatedOn { get; set; }

    /// <summary>
    ///     Gets or sets the name of the user who last modified this entity.
    /// </summary>
    public string ModifiedBy { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the date and time when this entity was last modified.
    /// </summary>
    public DateTime ModifiedOn { get; set; }
}