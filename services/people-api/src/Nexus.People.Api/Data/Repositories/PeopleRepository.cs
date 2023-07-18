using Microsoft.EntityFrameworkCore;
using Nexus.PeopleAPI.Entities;
using Nexus.Persistence;

namespace Nexus.PeopleAPI.Data.Repositories;

public class PeopleRepository : EfNexusRepository<Person>
{
    public PeopleRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsWithNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(x => x.Name == name, cancellationToken);
    }
    
    public async Task<bool> ExistsWithEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(x => x.Email == email, cancellationToken);
    }

    public async Task<Person?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
    }
    
    public async Task<Person?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
    }
}