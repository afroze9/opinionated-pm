using LanguageExt.Common;
using Nexus.CompanyAPI.Entities;

namespace Nexus.CompanyAPI.Abstractions;

/// <summary>
/// Provides operations for creating and retrieving tags.
/// </summary>
public interface ITagService
{
    /// <summary>
    ///     Creates a new tag with the specified name.
    /// </summary>
    /// <param name="name">The name of the new tag.</param>
    /// <returns>A task representing the asynchronous operation that returns the created tag DTO.</returns>
    Task<Result<Tag>> CreateAsync(string name);

    /// <summary>
    ///     Deletes the tag with the specified name.
    /// </summary>
    /// <param name="name">The name of the tag to delete.</param>
    /// <returns>
    ///     A task representing the asynchronous operation that returns a boolean indicating whether the deletion was
    ///     successful.
    /// </returns>
    Task<Result<bool>> DeleteAsync(string name);

    /// <summary>
    ///     Retrieves all tags.
    /// </summary>
    /// <returns>A task representing the asynchronous operation that returns a list of tag DTOs.</returns>
    Task<List<Tag>> GetAllAsync();

    Task<Result<Tag>> GetByIdAsync(int id);
}