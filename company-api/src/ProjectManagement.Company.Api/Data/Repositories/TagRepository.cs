using Microsoft.EntityFrameworkCore;
using ProjectManagement.CompanyAPI.Entities;
using ProjectManagement.Persistence;

namespace ProjectManagement.CompanyAPI.Data.Repositories;

public class TagRepository : EfCustomRepository<Tag>
{
    public TagRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsWithNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(x => x.Name == name, cancellationToken);
    }

    public async Task<Tag?> GetByName(string name)
    {
        return await DbSet.FirstOrDefaultAsync(x => x.Name == name);
    }
}