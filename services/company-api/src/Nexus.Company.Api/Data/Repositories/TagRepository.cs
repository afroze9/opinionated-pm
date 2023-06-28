using Microsoft.EntityFrameworkCore;
using Nexus.CompanyAPI.Entities;
using Nexus.Persistence;

namespace Nexus.CompanyAPI.Data.Repositories;

public class TagRepository : EfNexusRepository<Tag>
{
    public TagRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsWithNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(x => x.Name == name, cancellationToken);
    }

    public async Task<Tag?> GetByNameAsync(string name)
    {
        return await DbSet.FirstOrDefaultAsync(x => x.Name == name);
    }
}