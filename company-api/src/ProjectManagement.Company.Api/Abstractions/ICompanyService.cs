using ProjectManagement.CompanyAPI.DTO;

namespace ProjectManagement.CompanyAPI.Abstractions;

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
    Task<List<CompanySummaryDto>> GetAllAsync();

    /// <summary>
    ///     Creates a new company asynchronously.
    /// </summary>
    /// <param name="companySummary">A company summary DTO containing the information of the new company to be created.</param>
    /// <returns>
    ///     A task representing the asynchronous operation. The result of the task contains the company summary DTO of the
    ///     newly created company.
    /// </returns>
    Task<CompanySummaryDto> CreateAsync(CompanySummaryDto companySummary);

    /// <summary>
    ///     Retrieves a company by its ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the company to retrieve.</param>
    /// <returns>
    ///     A task representing the asynchronous operation. The result of the task contains the company DTO if found,
    ///     otherwise null.
    /// </returns>
    Task<CompanyDto?> GetByIdAsync(int id);

    /// <summary>
    ///     Updates the name of a company asynchronously.
    /// </summary>
    /// <param name="id">The ID of the company to update.</param>
    /// <param name="name">The new name of the company.</param>
    /// <returns>
    ///     A task representing the asynchronous operation. The result of the task contains the updated company summary
    ///     DTO if successful, otherwise null.
    /// </returns>
    Task<CompanySummaryDto?> UpdateNameAsync(int id, string name);

    /// <summary>
    ///     Adds a tag to a company asynchronously.
    /// </summary>
    /// <param name="id">The ID of the company to add the tag to.</param>
    /// <param name="tagName">The name of the tag to add.</param>
    /// <returns>
    ///     A task representing the asynchronous operation. The result of the task contains the updated company summary
    ///     DTO if successful, otherwise null.
    /// </returns>
    Task<CompanySummaryDto?> AddTagAsync(int id, string tagName);

    /// <summary>
    ///     Deletes a tag from a company asynchronously.
    /// </summary>
    /// <param name="id">The ID of the company to delete the tag from.</param>
    /// <param name="tagName">The name of the tag to delete.</param>
    /// <returns>
    ///     A task representing the asynchronous operation. The result of the task contains the updated company summary
    ///     DTO if successful, otherwise null.
    /// </returns>
    Task<CompanySummaryDto?> DeleteTagAsync(int id, string tagName);

    /// <summary>
    ///     Deletes a company by its ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the company to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(int id);
}