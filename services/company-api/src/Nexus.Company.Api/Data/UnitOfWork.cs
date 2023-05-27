using Nexus.CompanyAPI.Data.Repositories;
using Nexus.Persistence;

namespace Nexus.CompanyAPI.Data;

public class UnitOfWork : UnitOfWorkBase
{
    public UnitOfWork(ApplicationDbContext context,
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