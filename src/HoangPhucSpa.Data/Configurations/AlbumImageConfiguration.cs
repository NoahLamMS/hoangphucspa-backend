using HoangPhucSpa.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HoangPhucSpa.Data.Configurations;

public sealed class AlbumImageConfiguration : IEntityTypeConfiguration<AlbumImage>
{
    public void Configure(EntityTypeBuilder<AlbumImage> b)
    {
        b.Property(x => x.ImageUrl).HasMaxLength(500);
        b.Property(x => x.Caption).HasMaxLength(300);
        b.Property(x => x.IsVisible).HasDefaultValue(true);
        b.HasIndex(x => new { x.AlbumId, x.IsVisible, x.SortOrder });
    }
}

