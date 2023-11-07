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
    private readonly ResiliencePipeline<HttpResponseMessage> _pipeline;

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
        _pipeline = resiliencePipelineProvider.GetPipeline<HttpResponseMessage>(PollyExtensions.ProjectsPipeline);
    }

    /// <summary>
    ///     Gets projects by company ID asynchronously.
    /// </summary>
    /// <param name="companyId">The ID of the company to get projects for.</param>
    /// <returns>A list of projects for the specified company.</returns>
    public async Task<List<ProjectSummaryDto>> GetProjectsByCompanyIdAsync(int companyId)
    {
        HttpResponseMessage? response = await _pipeline
            .ExecuteAsync<HttpResponseMessage>(async (ct) => await _client.GetAsync($"https://project-api/api/v1/Project?companyId={companyId}", ct))
            .ConfigureAwait(false);
        
        if (!response.IsSuccessStatusCode)
        {
            return new List<ProjectSummaryDto>();
        }

        List<ProjectResponseModel>? projects =
            await response.Content.ReadFromJsonAsync<List<ProjectResponseModel>>();

        return _mapper.Map<List<ProjectSummaryDto>>(projects);
    }
}
