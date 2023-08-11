using Nexus.PeopleAPI.Data.Repositories;
using Nexus.Persistence;

namespace Nexus.PeopleAPI.Data;

public class UnitOfWork : UnitOfWorkBase
{
    public UnitOfWork(ApplicationDbContext context,
        PeopleRepository peopleRepository,
        SyncStatusRepository syncStatusRepository)
        : base(context)
    {
        People = peopleRepository;
        SyncStatuses = syncStatusRepository;
    }

    public PeopleRepository People { get; }
    
    public SyncStatusRepository SyncStatuses { get; }
}