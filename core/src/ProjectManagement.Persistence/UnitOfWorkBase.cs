using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ProjectManagement.Persistence.Abstractions;

namespace ProjectManagement.Persistence;

public class UnitOfWorkBase : IUnitOfWork
{
    private readonly DbContext _context;
    private IDbContextTransaction? _transaction;

    protected UnitOfWorkBase(DbContext context)
    {
        _context = context;
    }

    public void BeginTransaction()
    {
        _transaction = _context.Database.BeginTransaction();
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context?.Dispose();
    }

    public void Commit()
    {
        try
        {
            _context.SaveChanges();
            _transaction?.Commit();
        }
        catch
        {
            Rollback();
            throw;
        }
    }

    public void Rollback()
    {
        _transaction?.Rollback();
    }
}