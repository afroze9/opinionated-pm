using Microsoft.EntityFrameworkCore;
using ProjectManagement.CompanyAPI.Domain.Entities;
using ProjectManagement.Persistence;

namespace ProjectManagement.CompanyAPI.Data;

public class CompanyUnitOfWork : UnitOfWorkBase
{
    public CompanyUnitOfWork(ApplicationDbContext context,
        CompanyRepository companyRepository,
        TagRepository tagRepository)
        : base(context)
    {
        Companies = companyRepository;
        Tags = tagRepository;
    }

    public CompanyRepository Companies { get; }

    public TagRepository Tags { get; }
}

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

public class CompanyRepository : EfCustomRepository<Company>
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
}