using System.Diagnostics;
using System.Diagnostics.Metrics;
using Nexus.Common.Abstractions;
using Nexus.Common.Attributes;

namespace Nexus.PeopleAPI.Telemetry;

[NexusService<IPeopleInstrumentation>(NexusServiceLifeTime.Singleton)]
[NexusMeter(MeterName)]
public class PeopleInstrumentation : IPeopleInstrumentation, IDisposable
{
    private readonly Meter _meter;
    internal const string ActivitySourceName = "PeopleApi.People";
    internal const string MeterName = "PeopleApi.People";
    
    public PeopleInstrumentation()
    {
        
        string? version = typeof(PeopleInstrumentation).Assembly.GetName().Version?.ToString();
        ActivitySource = new ActivitySource(ActivitySourceName, version);
        _meter = new Meter(MeterName, version);
        GetAllPeopleCounter =
            _meter.CreateCounter<long>("PeopleApi.getall", "The number of calls to GetAllPeople endpoint");
    }
    
    public ActivitySource ActivitySource { get; }
    
    public Counter<long> GetAllPeopleCounter { get; }

    public void Dispose()
    {
        ActivitySource.Dispose();
        _meter.Dispose();
    }
}

public interface IPeopleInstrumentation : INexusService
{
    ActivitySource ActivitySource { get; }

    Counter<long> GetAllPeopleCounter { get; }
}