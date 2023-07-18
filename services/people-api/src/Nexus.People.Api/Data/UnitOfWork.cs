using Nexus.PeopleAPI.Data.Repositories;
using Nexus.Persistence;

namespace Nexus.PeopleAPI.Data;

public class UnitOfWork : UnitOfWorkBase
{
    public UnitOfWork(ApplicationDbContext context,
        PeopleRepository peopleRepository)
        : base(context)
    {
        People = peopleRepository;
    }

    public PeopleRepository People { get; }
}