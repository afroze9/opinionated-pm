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
    
    public async Task<bool> ExistsWithIdentityIdAsync(string identityId, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(x => x.IdentityId == identityId, cancellationToken);
    }
    
    public async Task<bool> AnyOtherPeopleWithSameEmailAsync(int id, string email)
    {
        return await DbSet.AnyAsync(x => x.Id != id && x.Email.ToLower() == email.ToLower());
    }

    public async Task<List<Person>> GetAllByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(x => x.Name.ToLower().Contains(name.ToLower())).ToListAsync(cancellationToken);
    }
    
    public async Task<Person?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
    }
}