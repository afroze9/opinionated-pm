namespace Nexus.CompanyAPI.Config;

public class ResilienceOptions
{
    public double FailureRatio { get; set; } = 0.5;

    public int MinimumThroughput { get; set; } = 10;

    public int SamplingDurationSeconds { get; set; } = 10;

    public int BreakDurationSeconds { get; set; } = 30;
}