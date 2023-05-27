using Nexus.Persistence.Auditing;

namespace Nexus.Framework.Web.Services;

public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;

    public DateTime UtcNow => DateTime.UtcNow;
}