using HoangPhucSpa.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HoangPhucSpa.Data.Configurations;

public sealed class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> b)
    {
        b.Property(x => x.Name).HasMaxLength(200);
        b.Property(x => x.Address).HasMaxLength(500);
        b.Property(x => x.Phone).HasMaxLength(50);
        b.Property(x => x.ZaloPhone).HasMaxLength(50);
        b.Property(x => x.FacebookUrl).HasMaxLength(500);
        b.Property(x => x.FacebookHandle).HasMaxLength(100);
        b.Property(x => x.MapUrl).HasMaxLength(1000);
        b.Property(x => x.MapEmbedUrl).HasMaxLength(2000);
        b.Property(x => x.OpeningHours).HasMaxLength(200);
        b.Property(x => x.Latitude).HasPrecision(9, 6);
        b.Property(x => x.Longitude).HasPrecision(9, 6);
        b.Property(x => x.IsVisible).HasDefaultValue(true);
        b.Property(x => x.RowVersion).IsConcurrencyToken();
        b.HasIndex(x => new { x.IsVisible, x.SortOrder });
    }
}

