using ProjectManagement.Persistence.Auditing;

namespace ProjectManagement.CompanyAPI.Services;

public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;

    public DateTime UtcNow => DateTime.UtcNow;
}