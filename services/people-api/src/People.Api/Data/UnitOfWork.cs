using Nexus.Persistence;
using PeopleAPI.Data.Repositories;

namespace PeopleAPI.Data;

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