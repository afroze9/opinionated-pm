﻿using Microsoft.EntityFrameworkCore;
using Nexus.Common.Attributes;
using Nexus.Persistence;
using Nexus.ProjectAPI.Entities;

namespace Nexus.ProjectAPI.Data.Repositories;

[NexusService(NexusServiceLifeTime.Scoped)]
public class ProjectRepository : EfNexusRepository<Project>
{
    public ProjectRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> AnyByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(x => x.Name == name, cancellationToken);
    }

    public async Task<Project?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
    }

    public async Task<List<Project>> GetAllByCompanyIdAsync(
        int? companyId,
        bool includeTags,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Project> query = DbSet.AsQueryable();

        if (includeTags)
        {
            query = query.Include(x => x.TodoItems);
        }

        if (companyId.HasValue)
        {
            query = query.Where(x => x.CompanyId == companyId.Value);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<Project?> GetByIdAsync(int id, bool includeTags, CancellationToken cancellationToken = default)
    {
        IQueryable<Project> query = DbSet.AsQueryable();

        if (includeTags)
        {
            query = query.Include(x => x.TodoItems);
        }

        return await query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}