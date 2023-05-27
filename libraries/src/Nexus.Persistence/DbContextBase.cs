using Microsoft.EntityFrameworkCore;

namespace Nexus.Persistence;

public class DbContextBase : DbContext
{
    protected DbContextBase(DbContextOptions options)
        : base(options)
    {
    }
}