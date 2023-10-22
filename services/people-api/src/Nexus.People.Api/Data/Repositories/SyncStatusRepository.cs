using Microsoft.EntityFrameworkCore;
using Nexus.Common.Attributes;
using Nexus.PeopleAPI.Entities;
using Nexus.Persistence;

namespace Nexus.PeopleAPI.Data.Repositories;

[NexusService(NexusServiceLifeTime.Scoped)]
public class SyncStatusRepository : EfNexusRepository<SyncStatus>
{
    public SyncStatusRepository(ApplicationDbContext context) : base(context)
    {
    }
    
    public async Task<SyncStatus?> GetByJobNameAsync(string jobName, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(x => x.JobName == jobName, cancellationToken);
    }
}