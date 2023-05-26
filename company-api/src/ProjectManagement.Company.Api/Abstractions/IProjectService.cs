using ProjectManagement.CompanyAPI.DTO;

namespace ProjectManagement.CompanyAPI.Abstractions;

/// <summary>
///     Interface for managing projects.
/// </summary>
public interface IProjectService
{
    /// <summary>
    ///     Gets projects by company ID asynchronously.
    /// </summary>
    /// <param name="companyId">The ID of the company to get projects for.</param>
    /// <returns>A list of projects for the specified company.</returns>
    Task<List<ProjectSummaryDto>> GetProjectsByCompanyIdAsync(int companyId);
}