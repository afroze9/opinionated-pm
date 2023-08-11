using LanguageExt.Common;
using Nexus.CompanyAPI.DTO;
using Nexus.CompanyAPI.Entities;

namespace Nexus.CompanyAPI.Abstractions;

/// <summary>
///     Provides methods for managing company information.
/// </summary>
public interface ICompanyService
{
    /// <summary>
    ///     Retrieves a list of all companies asynchronously.
    /// </summary>
    /// <returns>
    ///     A task representing the asynchronous operation. The result of the task contains a list of company summary
    ///     DTOs.
    /// </returns>
    Task<Result<List<CompanySummaryDto>>> GetAllAsync();

    /// <summary>
    ///     Creates a new company asynchronously.
    /// </summary>
    /// <param name="company">The new company to be created.</param>
    /// <returns>
    ///     A task representing the asynchronous operation. The result of the task contains the company summary DTO of the
    ///     newly created company.
    /// </returns>
    Task<Result<Company>> CreateAsync(Company company);

    /// <summary>
    ///     Retrieves a company by its ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the company to retrieve.</param>
    /// <returns>
    ///     A task representing the asynchronous operation. The result of the task contains the company DTO if found,
    ///     otherwise null.
    /// </returns>
    Task<Result<CompanyDto>> GetByIdAsync(int id);

    /// <summary>
    ///     Updates the name of a company asynchronously.
    /// </summary>
    /// <param name="id">The ID of the company to update.</param>
    /// <param name="name">The new name of the company.</param>
    /// <returns>
    ///     A task representing the asynchronous operation. The result of the task contains the updated company summary
    ///     DTO if successful, otherwise null.
    /// </returns>
    Task<Result<Company>> UpdateNameAsync(int id, string name);

    /// <summary>
    ///     Adds a tag to a company asynchronously.
    /// </summary>
    /// <param name="id">The ID of the company to add the tag to.</param>
    /// <param name="tagName">The name of the tag to add.</param>
    /// <returns>
    ///     A task representing the asynchronous operation. The result of the task contains the updated company summary
    ///     DTO if successful, otherwise null.
    /// </returns>
    Task<Result<Company>> AddTagAsync(int id, string tagName);

    /// <summary>
    ///     Deletes a tag from a company asynchronously.
    /// </summary>
    /// <param name="id">The ID of the company to delete the tag from.</param>
    /// <param name="tagName">The name of the tag to delete.</param>
    /// <returns>
    ///     A task representing the asynchronous operation. The result of the task contains the updated company summary
    ///     DTO if successful, otherwise null.
    /// </returns>
    Task<Result<Company>> DeleteTagAsync(int id, string tagName);

    /// <summary>
    ///     Deletes a company by its ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the company to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<Result<bool>> DeleteAsync(int id);
}