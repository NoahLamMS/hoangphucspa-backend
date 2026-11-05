using HoangPhucSpa.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HoangPhucSpa.Data.Configurations;

public sealed class PostCategoryConfiguration : IEntityTypeConfiguration<PostCategory>
{
    public void Configure(EntityTypeBuilder<PostCategory> b)
    {
        b.Property(x => x.Name).HasMaxLength(200);
        b.Property(x => x.Slug).HasMaxLength(200);
        b.HasIndex(x => new { x.Kind, x.Slug }).IsUnique();
        b.Property(x => x.Summary).HasMaxLength(1000);
        b.Property(x => x.SeoTitle).HasMaxLength(200);
        b.Property(x => x.SeoDescription).HasMaxLength(500);
        b.Property(x => x.SeoKeywords).HasMaxLength(500);
        b.Property(x => x.ThumbnailUrl).HasMaxLength(500);
        b.Property(x => x.SearchText).HasMaxLength(2000);
        b.Property(x => x.IsVisible).HasDefaultValue(true);
        b.Property(x => x.RowVersion).IsConcurrencyToken();
        b.HasIndex(x => new { x.Kind, x.IsVisible, x.SortOrder });

        b.HasMany(x => x.Posts)
            .WithOne(x => x.Category)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

