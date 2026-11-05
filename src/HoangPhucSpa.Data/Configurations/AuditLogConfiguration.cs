using HoangPhucSpa.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HoangPhucSpa.Data.Configurations;

public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> b)
    {
        b.Property(x => x.Action).HasMaxLength(50);
        b.Property(x => x.EntityType).HasMaxLength(100);
        b.Property(x => x.EntityId).HasMaxLength(100);
        b.Property(x => x.UserName).HasMaxLength(200);
        b.Property(x => x.IpAddress).HasMaxLength(64);
        b.Property(x => x.Summary).HasMaxLength(1000);
        b.HasIndex(x => x.OccurredAt);
    }
}

