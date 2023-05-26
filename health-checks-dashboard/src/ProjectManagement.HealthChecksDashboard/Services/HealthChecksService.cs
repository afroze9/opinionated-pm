using Microsoft.Extensions.Options;
using ProjectManagement.HealthChecksDashboard.Abstractions;
using ProjectManagement.HealthChecksDashboard.Configuration;
using ProjectManagement.HealthChecksDashboard.Entities;

namespace ProjectManagement.HealthChecksDashboard.Services;

public class HealthChecksService : IHealthChecksService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HealthChecksService> _logger;
    private readonly IApplicationDbContext _context;
    private readonly HealthCheckOptions _options;

    public HealthChecksService(
        IHttpClientFactory httpClient,
        ILogger<HealthChecksService> logger,
        IOptionsSnapshot<HealthCheckOptions> settings,
        IApplicationDbContext context)
    {
        _httpClient = httpClient.CreateClient("healthchecks");
        _logger = logger;
        _context = context;
        _options = settings.Value;
    }

    public async Task CheckHealthAsync()
    {
        foreach (HealthCheckClient client in _options.Clients)
        {
            _logger.LogInformation("Checking health for {Name}...", client.Name);

            try
            {
                _logger.LogInformation("Getting status for {Name} and {Url}...", client.Name, client.Url);
                HttpResponseMessage response = await _httpClient.GetAsync(client.Url);
                string json = await response.Content.ReadAsStringAsync();

                HealthCheckRecord healthCheckRecord = new ()
                {
                    ClientName = client.Name,
                    CreatedAt = DateTime.UtcNow,
                    Response = json,
                };

                await _context.HealthCheckRecords.AddAsync(healthCheckRecord);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                await _context.HealthCheckRecords.AddAsync(new HealthCheckRecord
                {
                    ClientName = client.Name,
                    CreatedAt = DateTime.UtcNow,
                    Response = "{\"status\":\"DOWN\"}",
                });

                await _context.SaveChangesAsync();
                _logger.LogWarning("Unable to get status for {clientName}. {error}", client.Name, e.ToString());
            }
        }
    }
}
