using Ardalis.Specification.EntityFrameworkCore;
using ProjectManagement.Core.Abstractions.Common;
using ProjectManagement.Persistence.Abstractions;

namespace ProjectManagement.CompanyAPI.Data;

[ExcludeFromCodeCoverage]
public class EfRepositoryBase<T> : RepositoryBase<T>, IReadRepository<T>, IRepository<T>
    where T : class, IEntity
{
    public EfRepositoryBase(ApplicationDbContext context)
        : base(context)
    {
    }
}