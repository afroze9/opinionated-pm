using AutoMapper;
using Nexus.Common.Attributes;
using Polly;
using Nexus.CompanyAPI.Abstractions;
using Nexus.CompanyAPI.DTO;
using Nexus.CompanyAPI.Resilience;
using Nexus.SharedKernel.Contracts.Project;
using Polly.Registry;

namespace Nexus.CompanyAPI.Services;

/// <summary>
///     Service for managing projects.
/// </summary>
[NexusTypedClient<IProjectService>("projects")]
public class ProjectService : IProjectService
{
    private readonly HttpClient _client;
    private readonly IMapper _mapper;
    private readonly ILogger<ProjectService> _logger;
    private readonly ResiliencePipeline<List<ProjectSummaryDto>> _pipeline;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ProjectService" /> class.
    /// </summary>
    /// <param name="client">The HTTP client.</param>
    /// <param name="mapper">The mapper.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="resiliencePipelineProvider">The resilience pipeline provider</param>
    public ProjectService(HttpClient client, IMapper mapper, 
        ILogger<ProjectService> logger, 
        ResiliencePipelineProvider<string> resiliencePipelineProvider)
    {
        _client = client;
        _mapper = mapper;
        _logger = logger;
        _pipeline = resiliencePipelineProvider.GetPipeline<List<ProjectSummaryDto>>(PollyExtensions.ProjectsPipeline);
    }

    /// <summary>
    ///     Gets projects by company ID asynchronously.
    /// </summary>
    /// <param name="companyId">The ID of the company to get projects for.</param>
    /// <param name="cancellationToken">Cancellation Token.</param>
    /// <returns>A list of projects for the specified company.</returns>
    public async Task<List<ProjectSummaryDto>> GetProjectsByCompanyIdAsync(int companyId, CancellationToken cancellationToken = default)
    {
        ResilienceContext context = ResilienceContextPool.Shared.Get(cancellationToken);
        Outcome<List<ProjectSummaryDto>> outcome = await _pipeline.ExecuteOutcomeAsync(
            DoGetProjectsByCompanyIdAsync, 
            context, 
            new GetProjectsByCompanyIdState(_client, _mapper, companyId));

        ResilienceContextPool.Shared.Return(context);
        
        if (outcome.Exception is not null)
        {
            _logger.LogWarning("Failed to get projects for company {CompanyId}", companyId);
            return new List<ProjectSummaryDto>();
        }

        return outcome.Result ?? new List<ProjectSummaryDto>();
    }

    private static async ValueTask<Outcome<List<ProjectSummaryDto>>> DoGetProjectsByCompanyIdAsync(ResilienceContext context, GetProjectsByCompanyIdState state)
    {
        try
        {
            using (HttpResponseMessage result = await state.Client.GetAsync($"https://project-api/api/v1/Project?companyId={state.CompanyId}", context.CancellationToken))
            {
                if (result is not { IsSuccessStatusCode: true })
                {
                    return Outcome.FromResult(new List<ProjectSummaryDto>());
                }

                List<ProjectResponseModel>? projects = await result.Content.ReadFromJsonAsync<List<ProjectResponseModel>>(cancellationToken: context.CancellationToken);
                List<ProjectSummaryDto> mappedProjects = state.Mapper.Map<List<ProjectSummaryDto>>(projects) ?? new List<ProjectSummaryDto>();
                return Outcome.FromResult(mappedProjects);
            }
        }
        catch (Exception ex)
        {
            return Outcome.FromException<List<ProjectSummaryDto>>(ex);
        }
    }
}

public record GetProjectsByCompanyIdState(HttpClient Client, IMapper Mapper, int CompanyId);
