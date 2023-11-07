using System.Net;
using Nexus.CompanyAPI.Config;
using Nexus.SharedKernel.Contracts.Project;
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
        
        CircuitBreakerStrategyOptions<HttpResponseMessage> circuitBreakerOptions = new()
        {
            FailureRatio = resilienceOptions.FailureRatio,
            SamplingDuration = TimeSpan.FromSeconds(resilienceOptions.SamplingDurationSeconds),
            MinimumThroughput = resilienceOptions.MinimumThroughput,
            BreakDuration = TimeSpan.FromSeconds(resilienceOptions.BreakDurationSeconds),
            ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                .Handle<Exception>(),
        };

        FallbackStrategyOptions<HttpResponseMessage> fallbackOptions = new()
        {
            ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                .Handle<Exception>(),
            FallbackAction = static args => Outcome.FromResultAsValueTask(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new List<ProjectResponseModel>()),
            }),
        };

        services.AddResiliencePipeline<string, HttpResponseMessage>(ProjectsPipeline, builder =>
        {
            builder
                .AddFallback(fallbackOptions)
                .AddCircuitBreaker(circuitBreakerOptions);
        });
    }
}