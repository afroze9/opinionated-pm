using ProjectManagement.CompanyAPI.Data.Repositories;
using ProjectManagement.Persistence;

namespace ProjectManagement.CompanyAPI.Data;

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