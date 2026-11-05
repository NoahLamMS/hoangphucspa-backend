using HoangPhucSpa.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HoangPhucSpa.Data.Configurations;

public sealed class AlbumConfiguration : IEntityTypeConfiguration<Album>
{
    public void Configure(EntityTypeBuilder<Album> b)
    {
        b.Property(x => x.Name).HasMaxLength(300);
        b.Property(x => x.Slug).HasMaxLength(300);
        b.HasIndex(x => x.Slug).IsUnique();
        b.Property(x => x.Summary).HasMaxLength(1000);
        b.Property(x => x.ThumbnailUrl).HasMaxLength(500);
        b.Property(x => x.SearchText).HasMaxLength(2000);
        b.Property(x => x.IsVisible).HasDefaultValue(true);
        b.Property(x => x.RowVersion).IsConcurrencyToken();
        b.HasIndex(x => new { x.IsVisible, x.SortOrder });

        b.HasMany(x => x.Images)
            .WithOne(x => x.Album)
            .HasForeignKey(x => x.AlbumId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

