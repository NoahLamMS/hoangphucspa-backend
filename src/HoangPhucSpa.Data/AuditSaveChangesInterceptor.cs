using HoangPhucSpa.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace HoangPhucSpa.Data;

/// <summary>
/// Interceptor tự động điền thông tin người tạo/cập nhật, thời gian UTC và cấp mới RowVersion (Optimistic Concurrency) trước khi lưu vào PostgreSQL.
/// </summary>
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

            foreach (var entry in eventData.Context.ChangeTracker.Entries<AuditableEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = now;
                    entry.Entity.CreatedById = userId;
                }

                if (entry.State is EntityState.Added or EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = now;
                    entry.Entity.UpdatedById = userId;
                    // Sinh mã RowVersion mới để EF Core kiểm tra xung đột ghi đồng thời (Optimistic Concurrency) trên PostgreSQL.
                    entry.Entity.RowVersion = Guid.NewGuid().ToByteArray();
                }
            }
        }

        return base.SavingChangesAsync(eventData, result, ct);
    }
}
