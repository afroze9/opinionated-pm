using Ardalis.Specification.EntityFrameworkCore;
using ProjectManagement.Core.Abstractions.Common;
using ProjectManagement.Persistence.Abstractions;

namespace ProjectManagement.ProjectAPI.Data;

[ExcludeFromCodeCoverage]
public class EfRepository<T> : RepositoryBase<T>, IReadRepository<T>, IRepository<T>
    where T : class, IEntity
{
    public EfRepository(ApplicationDbContext context)
        : base(context)
    {
    }
}