using HoangPhucSpa.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HoangPhucSpa.Data.Configurations;

public sealed class SlideConfiguration : IEntityTypeConfiguration<Slide>
{
    public void Configure(EntityTypeBuilder<Slide> b)
    {
        b.Property(x => x.Title).HasMaxLength(200);
        b.Property(x => x.AltText).HasMaxLength(300);
        b.Property(x => x.ImageUrl).HasMaxLength(500);
        b.Property(x => x.LinkUrl).HasMaxLength(500);
        b.Property(x => x.IsVisible).HasDefaultValue(true);
        b.Property(x => x.RowVersion).IsConcurrencyToken();
        b.HasIndex(x => new { x.IsVisible, x.SortOrder });
    }
}

