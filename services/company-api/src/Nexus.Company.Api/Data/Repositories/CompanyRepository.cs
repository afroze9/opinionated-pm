using Microsoft.EntityFrameworkCore;
using Nexus.Common.Attributes;
using Nexus.CompanyAPI.Entities;
using Nexus.Persistence;

namespace Nexus.CompanyAPI.Data.Repositories;

[NexusService(NexusServiceLifeTime.Scoped)]
public class CompanyRepository : EfNexusRepository<Company>
{
    public CompanyRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<Company>> AllCompaniesWithTagsAsync()
    {
        return await DbSet.Include(x => x.Tags).ToListAsync();
    }

    public async Task<bool> ExistsWithNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(x => x.Name == name, cancellationToken);
    }

    public async Task<Company?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
    }

    public async Task<bool> AnyOtherCompaniesWithSameNameAsync(int id, string name)
    {
        return await DbSet.AnyAsync(x => x.Id != id && x.Name == name);
    }

    public async Task<bool> AnyCompaniesExistWithTagName(string tagName, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(x => x.Tags.Any(t => t.Name == tagName), cancellationToken);
    }

    public async Task<Company?> GetByIdWithTagsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(x => x.Tags)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}