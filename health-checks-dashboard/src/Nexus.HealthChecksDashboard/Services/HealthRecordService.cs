using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Nexus.HealthChecksDashboard.Abstractions;
using Nexus.HealthChecksDashboard.Configuration;
using Nexus.HealthChecksDashboard.Converters;
using Nexus.HealthChecksDashboard.Data;
using Nexus.HealthChecksDashboard.Entities;
using Nexus.HealthChecksDashboard.Models;

namespace Nexus.HealthChecksDashboard.Services;

public class HealthRecordService : IHealthRecordService
{
    private readonly ApplicationDbContext _context;
    private readonly HealthCheckOptions _healthCheckOptions;
    private readonly JsonSerializerSettings _jsonSerializerSettings;

    public HealthRecordService(
        ApplicationDbContext context,
        IOptionsSnapshot<HealthCheckOptions> healthCheckOptions)
    {
        _context = context;
        _healthCheckOptions = healthCheckOptions.Value;
        _jsonSerializerSettings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> { new HealthCheckResponseConverter() },
        };
    }

    public async Task<HealthRecordModel[]> GetHealthRecordsAsync(CancellationToken cancellationToken = default)
    {
        if (_healthCheckOptions.Clients == null || _healthCheckOptions.Clients.Length == 0)
        {
            return Array.Empty<HealthRecordModel>();
        }

        List<HealthRecordModel> healthRecords = new ();

        foreach (HealthCheckClient client in _healthCheckOptions.Clients)
        {
            HealthCheckRecord? clientRecord = await _context.HealthCheckRecords
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(x => x.ClientName == client.Name, cancellationToken);

            if (clientRecord != null)
            {
                healthRecords.Add(new HealthRecordModel
                {
                    CreatedAt = clientRecord.CreatedAt,
                    Response = string.IsNullOrEmpty(clientRecord.Response)
                        ? null
                        : JsonConvert.DeserializeObject<HealthCheckResponse>(clientRecord.Response,
                            _jsonSerializerSettings),
                    ClientName = client.Name,
                });
            }
            else
            {
                healthRecords.Add(new HealthRecordModel
                {
                    CreatedAt = DateTime.UtcNow,
                    ClientName = client.Name,
                    Response = new HealthCheckResponse
                    {
                        Status = "DOWN",
                    },
                });
            }
        }

        return healthRecords.ToArray();
    }
}