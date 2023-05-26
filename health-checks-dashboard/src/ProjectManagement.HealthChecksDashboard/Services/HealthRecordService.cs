using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ProjectManagement.HealthChecksDashboard.Abstractions;
using ProjectManagement.HealthChecksDashboard.Configuration;
using ProjectManagement.HealthChecksDashboard.Converters;
using ProjectManagement.HealthChecksDashboard.Entities;
using ProjectManagement.HealthChecksDashboard.Models;

namespace ProjectManagement.HealthChecksDashboard.Data;

public class HealthRecordService : IHealthRecordService
{
    private readonly IApplicationDbContext _context;
    private readonly HealthCheckOptions _healthCheckOptions;
    private readonly JsonSerializerSettings _jsonSerializerSettings;

    public HealthRecordService(
        IApplicationDbContext context,
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
        if (_healthCheckOptions.Clients.Length == 0)
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