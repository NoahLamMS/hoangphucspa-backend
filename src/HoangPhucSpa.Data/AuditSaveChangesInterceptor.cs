using HoangPhucSpa.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace HoangPhucSpa.Data;

public sealed class AuditSaveChangesInterceptor(ICurrentUser user, TimeProvider clock) : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken ct = default)
    {
        if (eventData.Context is not null)
        {
            var now = clock.GetUtcNow().UtcDateTime;
            var userId = user.Id;

            foreach (var e in eventData.Context.ChangeTracker.Entries<AuditableEntity>())
            {
                if (e.State == EntityState.Added)
                {
                    e.Entity.CreatedAt = now;
                    e.Entity.CreatedById = userId;
                }
                if (e.State is EntityState.Added or EntityState.Modified)
                {
                    e.Entity.UpdatedAt = now;
                    e.Entity.UpdatedById = userId;
                }
            }
        }

        return base.SavingChangesAsync(eventData, result, ct);
    }
}
