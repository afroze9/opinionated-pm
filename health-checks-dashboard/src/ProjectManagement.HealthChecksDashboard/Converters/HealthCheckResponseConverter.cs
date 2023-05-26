using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProjectManagement.HealthChecksDashboard.Entities;

namespace ProjectManagement.HealthChecksDashboard.Converters;

public class HealthCheckResponseConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue,
        JsonSerializer serializer)
    {
        JObject jsonObject = JObject.Load(reader);

        string? status = (string?) jsonObject["status"];
        JObject? details = (JObject?) jsonObject["details"];

        if (status == null || details == null)
        {
            return null;
        }

        HealthCheckResponse response = new ()
        {
            Status = status,
        };

        foreach (KeyValuePair<string, JToken?> detail in details)
        {
            string serviceName = detail.Key;

            if (detail.Value == null || detail.Value.Type == JTokenType.Null)
            {
                continue;
            }

            JObject serviceDetails = (JObject?) detail.Value!;

            if (serviceDetails.TryGetValue("LivenessState", out JToken? livenessDetail))
            {
                response.Liveness = new LivenessDetail
                {
                    LivenessState = (string?) livenessDetail ?? "UNKNOWN",
                };
            }
            else if (serviceDetails.TryGetValue("ReadinessState", out JToken? readinessDetail))
            {
                response.Readiness = new ReadinessDetail
                {
                    ReadinessState = (string?) readinessDetail ?? "UNKNOWN",
                };
            }
            else if (serviceDetails.ContainsKey("status") && serviceDetails.ContainsKey("database"))
            {
                response.Database = new DatabaseDetail
                {
                    Database = (string?) serviceDetails["database"] ?? string.Empty,
                    Status = (string?) serviceDetails["status"] ?? "UNKNOWN",
                };
            }
            else if (serviceDetails.ContainsKey("leader") && serviceDetails.ContainsKey("services"))
            {
                response.Consul = new ConsulDetail
                {
                    Leader = (string?) serviceDetails["leader"] ?? string.Empty,
                    Services = serviceDetails["services"]?.ToObject<Dictionary<string, string[]>>() ??
                               new Dictionary<string, string[]>(),
                };
            }
            else if (serviceDetails.ContainsKey("total") && serviceDetails.ContainsKey("free")
                                                         && serviceDetails.ContainsKey("threshold")
                                                         && serviceDetails.ContainsKey("status"))
            {
                response.DiskSpace = new DiskSpaceDetail
                {
                    Total = (long?) serviceDetails["total"] ?? 0,
                    Free = (long?) serviceDetails["free"] ?? 0,
                    Threshold = (long?) serviceDetails["threshold"] ?? 0,
                    Status = (string?) serviceDetails["status"] ?? "UNKNOWN",
                };
            }
            else if (serviceName.ToLower() == "ping" && serviceDetails.Type == JTokenType.Integer)
            {
                response.Ping = new PingDetail
                {
                    Ping = serviceDetails.ToObject<int>(),
                };
            }
        }

        return response;
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(HealthCheckResponse);
    }
}