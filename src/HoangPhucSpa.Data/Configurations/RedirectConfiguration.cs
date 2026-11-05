using HoangPhucSpa.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HoangPhucSpa.Data.Configurations;

public sealed class RedirectConfiguration : IEntityTypeConfiguration<Redirect>
{
    public void Configure(EntityTypeBuilder<Redirect> b)
    {
        b.Property(x => x.FromPath).HasMaxLength(500);
        b.HasIndex(x => x.FromPath).IsUnique();
        b.Property(x => x.ToPath).HasMaxLength(500);
    }
}

