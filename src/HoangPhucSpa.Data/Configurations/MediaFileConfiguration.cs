using HoangPhucSpa.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HoangPhucSpa.Data.Configurations;

public sealed class MediaFileConfiguration : IEntityTypeConfiguration<MediaFile>
{
    public void Configure(EntityTypeBuilder<MediaFile> b)
    {
        b.Property(x => x.Path).HasMaxLength(500);
        b.HasIndex(x => x.Path).IsUnique();
        b.Property(x => x.OriginalFileName).HasMaxLength(300);
        b.Property(x => x.ContentType).HasMaxLength(100);
        b.Property(x => x.Sha256).HasMaxLength(64).IsUnicode(false);
        b.Property(x => x.AltText).HasMaxLength(300);
    }
}

