using HoangPhucSpa.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HoangPhucSpa.Data.Configurations;

public sealed class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> b)
    {
        b.Property(x => x.Title).HasMaxLength(500);
        b.Property(x => x.Slug).HasMaxLength(500);
        b.HasIndex(x => new { x.Kind, x.Slug }).HasFilter("\"DeletedAt\" IS NULL");
        b.Property(x => x.Summary).HasMaxLength(1000);
        b.Property(x => x.SeoTitle).HasMaxLength(200);
        b.Property(x => x.SeoDescription).HasMaxLength(500);
        b.Property(x => x.SeoKeywords).HasMaxLength(500);
        b.Property(x => x.ThumbnailUrl).HasMaxLength(500);
        b.Property(x => x.BannerUrl).HasMaxLength(500);
        b.Property(x => x.SearchText).HasMaxLength(4000);
        b.Property(x => x.IsVisible).HasDefaultValue(true);
        b.Property(x => x.RowVersion).IsConcurrencyToken();
        b.HasIndex(x => new { x.Kind, x.IsVisible, x.CategoryId, x.SortOrder, x.PublishedAt }).HasFilter("\"DeletedAt\" IS NULL");
        b.HasQueryFilter(x => x.DeletedAt == null);
    }
}

