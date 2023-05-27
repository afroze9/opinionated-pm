using Microsoft.Extensions.Options;
using Nexus.HealthChecksDashboard.Abstractions;
using Nexus.HealthChecksDashboard.Configuration;

namespace Nexus.HealthChecksDashboard.Services;

public class HealthCheckBackgroundService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<HealthCheckBackgroundService> _logger;
    private readonly HealthCheckOptions _healthCheckOptions;

    public HealthCheckBackgroundService(IServiceProvider services,
        ILogger<HealthCheckBackgroundService> logger,
        IOptions<HealthCheckOptions> healthCheckOptions)
    {
        _services = services;
        _logger = logger;
        _healthCheckOptions = healthCheckOptions.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using IServiceScope scope = _services.CreateScope();
            IHealthChecksService healthCheckService = scope.ServiceProvider.GetRequiredService<IHealthChecksService>();
            _logger.LogInformation("Checking health...");
            await healthCheckService.CheckHealthAsync();
            await Task.Delay(TimeSpan.FromSeconds(_healthCheckOptions.IntervalInSeconds), stoppingToken);
        }
    }
}