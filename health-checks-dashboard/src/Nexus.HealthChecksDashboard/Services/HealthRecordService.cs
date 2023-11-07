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

    public async Task<ServiceHealthRecordModel[]> GetHealthRecordsAsync(CancellationToken cancellationToken = default)
    {
        if (_healthCheckOptions.Clients == null || _healthCheckOptions.Clients.Length == 0)
        {
            return Array.Empty<ServiceHealthRecordModel>();
        }

        List<ServiceHealthRecordModel> healthRecords = new ();

        foreach (HealthCheckClient client in _healthCheckOptions.Clients)
        {
            ServiceHealthCheckRecord? serviceRecord = await _context.HealthCheckRecords
                .Include(x => x.InstanceRecords)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(x => x.ClientName == client.Name, cancellationToken);

            if (serviceRecord != null)
            {
                ServiceHealthRecordModel? serviceHcModel = new()
                {
                    CreatedAt = serviceRecord.CreatedAt,
                    ClientName = client.Name,
                };

                foreach (InstanceHealthCheckRecord instanceRecord in serviceRecord.InstanceRecords)
                {
                    serviceHcModel.InstanceHealthRecords.Add(new InstanceHealthRecordModel
                    {
                        CreatedAt = instanceRecord.CreatedAt,
                        Response = string.IsNullOrEmpty(instanceRecord.Response)
                            ? null
                            : JsonConvert.DeserializeObject<HealthCheckResponse>(instanceRecord.Response,
                                _jsonSerializerSettings),
                        InstanceNumber = instanceRecord.InstanceNumber,
                    });
                }

                healthRecords.Add(serviceHcModel);
            }
            else
            {
                healthRecords.Add(new ServiceHealthRecordModel
                {
                    CreatedAt = DateTime.UtcNow,
                    ClientName = client.Name,
                });
            }
        }

        return healthRecords.ToArray();
    }
}
