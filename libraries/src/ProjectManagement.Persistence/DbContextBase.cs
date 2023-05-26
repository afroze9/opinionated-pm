using Microsoft.EntityFrameworkCore;

namespace ProjectManagement.Persistence;

public class DbContextBase : DbContext
{
    protected DbContextBase(DbContextOptions options)
        : base(options)
    {
    }
}