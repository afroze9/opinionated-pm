using Microsoft.Extensions.Options;
using Nexus.HealthChecksDashboard.Abstractions;
using Nexus.HealthChecksDashboard.Configuration;
using Nexus.HealthChecksDashboard.Data;
using Nexus.HealthChecksDashboard.Entities;
using Steeltoe.Common.Discovery;
using Steeltoe.Discovery;

namespace Nexus.HealthChecksDashboard.Services;

public class HealthChecksService : IHealthChecksService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HealthChecksService> _logger;
    private readonly ApplicationDbContext _context;
    private readonly IDiscoveryClient _discoveryClient;
    private readonly HealthCheckOptions _options;

    private readonly string[] _healthCheckServiceTypes = { "rest-api", "framework" };
    private const string NexusServiceTypeTag = "nexus-service-type";

    public HealthChecksService(
        IHttpClientFactory httpClient,
        ILogger<HealthChecksService> logger,
        IOptionsSnapshot<HealthCheckOptions> settings,
        ApplicationDbContext context,
        IDiscoveryClient discoveryClient)
    {
        _httpClient = httpClient.CreateClient("healthchecks");
        _logger = logger;
        _context = context;
        _discoveryClient = discoveryClient;
        _options = settings.Value;
    }

    public async Task CheckHealthAsync()
    {
        if (_options.Clients == null || _options.Clients.Length == 0)
        {
            return;
        }

        foreach (HealthCheckClient client in _options.Clients)
        {
            ServiceHealthCheckRecord serviceRecord = new ()
            {
                ClientName = client.Name,
                CreatedAt = DateTime.UtcNow,
            };

            IList<IServiceInstance>? instances = await _discoveryClient.GetInstancesWithCacheAsync(client.ServiceName);
            List<IServiceInstance> filteredInstances = instances
                .Where(x => x.Metadata.ContainsKey(NexusServiceTypeTag) &&
                            _healthCheckServiceTypes.Contains(x.Metadata[NexusServiceTypeTag]))
                .DistinctBy(x => x.Uri)
                .ToList();

            if (!filteredInstances.Any())
            {
                await _context.HealthCheckRecords.AddAsync(
                    new ServiceHealthCheckRecord
                    {
                        ClientName = client.Name,
                        CreatedAt = DateTime.UtcNow,
                    });
            }
            else
            {
                for (int i = 0; i < filteredInstances.Count; i++)
                {
                    InstanceHealthCheckRecord instanceRecord =
                        await GetInstanceHealth(client.Name, filteredInstances[i].Uri, i);

                    serviceRecord.InstanceRecords.Add(instanceRecord);
                }
            }

            await _context.HealthCheckRecords.AddAsync(serviceRecord);
            await _context.SaveChangesAsync();
        }
    }

    private async Task<InstanceHealthCheckRecord> GetInstanceHealth(string clientName, Uri baseUri, int instanceNumber)
    {
        _logger.LogInformation("Checking health for {ClientName} instance {InstanceNumber}...", instanceNumber,
            clientName);

        Uri hcUri = new (baseUri, "actuator/health");

        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync(hcUri);
            string json = await response.Content.ReadAsStringAsync();
            return new InstanceHealthCheckRecord()
            {
                Response = json,
                CreatedAt = DateTime.UtcNow,
                InstanceNumber = instanceNumber + 1,
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Unable to get status for {ClientName} instance {InstanceNumber}. {Error}",
                clientName, instanceNumber, ex.ToString());

            return new InstanceHealthCheckRecord()
            {
                Response = "{\"status\":\"DOWN\"}",
                CreatedAt = DateTime.UtcNow,
                InstanceNumber = instanceNumber + 1,
            };
        }
    }
}
