using Nexus.CompanyAPI.Config;
using Nexus.CompanyAPI.DTO;
using Polly;
using Polly.CircuitBreaker;
using Polly.Fallback;

namespace Nexus.CompanyAPI.Resilience;

public static class PollyExtensions
{
    public const string ProjectsPipeline = "projects";
    public static void AddResilience(this IServiceCollection services, IConfiguration configuration)
    {
        ResilienceOptions resilienceOptions = new ();
        configuration.GetSection("Resilience").Bind(resilienceOptions);
        
        CircuitBreakerStrategyOptions<List<ProjectSummaryDto>> circuitBreakerOptions = new()
        {
            FailureRatio = resilienceOptions.FailureRatio,
            SamplingDuration = TimeSpan.FromSeconds(resilienceOptions.SamplingDurationSeconds),
            MinimumThroughput = resilienceOptions.MinimumThroughput,
            BreakDuration = TimeSpan.FromSeconds(resilienceOptions.BreakDurationSeconds),
            ShouldHandle = new PredicateBuilder<List<ProjectSummaryDto>>()
                .Handle<Exception>(),
        };

        FallbackStrategyOptions<List<ProjectSummaryDto>> fallbackOptions = new()
        {
            ShouldHandle = new PredicateBuilder<List<ProjectSummaryDto>>()
                .Handle<Exception>(),
            FallbackAction = static args => Outcome.FromResultAsValueTask(new List<ProjectSummaryDto>()),
        };

        services.AddResiliencePipeline<string, List<ProjectSummaryDto>>(ProjectsPipeline, builder =>
        {
            builder
                .AddFallback(fallbackOptions)
                .AddCircuitBreaker(circuitBreakerOptions);
        });
    }
}