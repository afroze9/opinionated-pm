using Ardalis.Specification.EntityFrameworkCore;
using ProjectManagement.CompanyAPI.Abstractions;

namespace ProjectManagement.CompanyAPI.Data;

/// <summary>
///     Implementation of <see cref="IRepository{T}" /> and <see cref="IReadRepository{T}" /> using Entity Framework Core.
/// </summary>
/// <typeparam name="T">The type of entity being operated on by this repository.</typeparam>
[ExcludeFromCodeCoverage]
public class EfRepository<T> : RepositoryBase<T>, IReadRepository<T>, IRepository<T>
    where T : class, IAggregateRoot
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="EfRepository{T}" /> class with the specified
    ///     <see cref="ApplicationDbContext" />.
    /// </summary>
    /// <param name="context">The <see cref="ApplicationDbContext" /> to be used by the repository.</param>
    public EfRepository(ApplicationDbContext context)
        : base(context)
    {
    }
}