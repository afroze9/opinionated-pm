using Microsoft.EntityFrameworkCore;
using ProjectManagement.Core.Abstractions.Common;
using ProjectManagement.Persistence.Abstractions;

namespace ProjectManagement.Persistence;

public class EfCustomRepository<T> : ICustomRepository<T>
    where T : class, IEntity
{
    protected readonly DbSet<T> DbSet;

    protected EfCustomRepository(DbContext context)
    {
        DbSet = context.Set<T>();
    }

    public T? GetById(int id)
    {
        return DbSet.Find(id);
    }

    public Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return DbSet.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public void Add(T entity)
    {
        DbSet.Add(entity);
    }

    public void Update(T entity)
    {
        DbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        DbSet.Remove(entity);
    }
}