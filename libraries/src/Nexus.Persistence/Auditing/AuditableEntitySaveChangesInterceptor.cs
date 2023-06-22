using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Nexus.Common;

namespace Nexus.Persistence.Auditing;

public class AuditableEntitySaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTime _dateTime;

    public AuditableEntitySaveChangesInterceptor(
        ICurrentUserService currentUserService,
        IDateTime dateTime)
    {
        _currentUserService = currentUserService;
        _dateTime = dateTime;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateEntities(DbContext? context)
    {
        if (context == null)
        {
            return;
        }

        foreach (EntityEntry<AuditableEntityBase> entry in context.ChangeTracker.Entries<AuditableEntityBase>())
        {
            if (entry.State == EntityState.Added)
            {
                //TODO: Check
                entry.Entity.CreatedBy = _currentUserService.UserId ?? string.Empty;
                entry.Entity.CreatedOn = _dateTime.UtcNow;
            }

            if (entry.State == EntityState.Added || entry.State == EntityState.Modified ||
                HasChangedOwnedEntities(entry))
            {
                //TODO: Check
                entry.Entity.ModifiedBy = _currentUserService.UserId ?? string.Empty;
                entry.Entity.ModifiedOn = _dateTime.UtcNow;
            }
        }
    }

    private static bool HasChangedOwnedEntities(EntityEntry entry)
    {
        return entry.References.Any(r =>
            r.TargetEntry != null &&
            r.TargetEntry.Metadata.IsOwned() &&
            (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
    }
}