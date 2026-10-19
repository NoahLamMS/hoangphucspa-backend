using HoangPhucSpa.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HoangPhucSpa.Data.Configurations;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> b)
    {
        b.Property(x => x.Name).HasMaxLength(200);
        b.Property(x => x.Slug).HasMaxLength(200);
        b.HasIndex(x => x.Slug).IsUnique().HasFilter("[DeletedAt] IS NULL");
        b.Property(x => x.Summary).HasMaxLength(1000);
        b.Property(x => x.SeoTitle).HasMaxLength(200);
        b.Property(x => x.SeoDescription).HasMaxLength(500);
        b.Property(x => x.SeoKeywords).HasMaxLength(500);
        b.Property(x => x.ThumbnailUrl).HasMaxLength(500);
        b.Property(x => x.BannerUrl).HasMaxLength(500);
        b.Property(x => x.SearchText).HasMaxLength(2000);
        b.Property(x => x.IsVisible).HasDefaultValue(true);
        b.Property(x => x.RowVersion).IsRowVersion();
        b.HasIndex(x => new { x.IsVisible, x.SortOrder });
        b.HasQueryFilter(x => x.DeletedAt == null);

        b.HasMany(x => x.SubServices)
            .WithOne(x => x.Service)
            .HasForeignKey(x => x.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class SubServiceConfiguration : IEntityTypeConfiguration<SubService>
{
    public void Configure(EntityTypeBuilder<SubService> b)
    {
        b.Property(x => x.Name).HasMaxLength(200);
        b.Property(x => x.Slug).HasMaxLength(200);
        b.HasIndex(x => new { x.ServiceId, x.Slug }).IsUnique().HasFilter("[DeletedAt] IS NULL");
        b.Property(x => x.Summary).HasMaxLength(1000);
        b.Property(x => x.SeoTitle).HasMaxLength(200);
        b.Property(x => x.SeoDescription).HasMaxLength(500);
        b.Property(x => x.SeoKeywords).HasMaxLength(500);
        b.Property(x => x.ThumbnailUrl).HasMaxLength(500);
        b.Property(x => x.BannerUrl).HasMaxLength(500);
        b.Property(x => x.SearchText).HasMaxLength(2000);
        b.Property(x => x.IsVisible).HasDefaultValue(true);
        b.Property(x => x.RowVersion).IsRowVersion();
        b.HasIndex(x => new { x.IsVisible, x.SortOrder });
        b.HasQueryFilter(x => x.DeletedAt == null);
    }
}

public class PostCategoryConfiguration : IEntityTypeConfiguration<PostCategory>
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
        b.Property(x => x.RowVersion).IsRowVersion();
        b.HasIndex(x => new { x.Kind, x.IsVisible, x.SortOrder });

        b.HasMany(x => x.Posts)
            .WithOne(x => x.Category)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> b)
    {
        b.Property(x => x.Name).HasMaxLength(300);
        b.Property(x => x.Slug).HasMaxLength(300);
        b.Property(x => x.PublicId).HasComputedColumnSql("COALESCE([LegacyId], [Id])", stored: true);
        b.HasIndex(x => new { x.Kind, x.PublicId }).IsUnique();
        b.HasIndex(x => new { x.Kind, x.LegacyId }).IsUnique().HasFilter("[LegacyId] IS NOT NULL");
        b.HasIndex(x => new { x.Kind, x.Slug }).HasFilter("[DeletedAt] IS NULL");
        b.Property(x => x.Summary).HasMaxLength(1000);
        b.Property(x => x.SeoTitle).HasMaxLength(200);
        b.Property(x => x.SeoDescription).HasMaxLength(500);
        b.Property(x => x.SeoKeywords).HasMaxLength(500);
        b.Property(x => x.ThumbnailUrl).HasMaxLength(500);
        b.Property(x => x.BannerUrl).HasMaxLength(500);
        b.Property(x => x.SearchText).HasMaxLength(3000);
        b.Property(x => x.IsVisible).HasDefaultValue(true);
        b.Property(x => x.RowVersion).IsRowVersion();
        b.HasIndex(x => new { x.Kind, x.IsVisible, x.PublishedAt });
        b.HasQueryFilter(x => x.DeletedAt == null);
    }
}

public class PageConfiguration : IEntityTypeConfiguration<Page>
{
    public void Configure(EntityTypeBuilder<Page> b)
    {
        b.Property(x => x.Key).HasMaxLength(100);
        b.HasIndex(x => x.Key).IsUnique();
        b.Property(x => x.Title).HasMaxLength(300);
        b.Property(x => x.Summary).HasMaxLength(1000);
        b.Property(x => x.SeoTitle).HasMaxLength(200);
        b.Property(x => x.SeoDescription).HasMaxLength(500);
        b.Property(x => x.SeoKeywords).HasMaxLength(500);
        b.Property(x => x.ThumbnailUrl).HasMaxLength(500);
        b.Property(x => x.BannerUrl).HasMaxLength(500);
        b.Property(x => x.SearchText).HasMaxLength(3000);
        b.Property(x => x.RowVersion).IsRowVersion();
    }
}

public class SlideConfiguration : IEntityTypeConfiguration<Slide>
{
    public void Configure(EntityTypeBuilder<Slide> b)
    {
        b.Property(x => x.Title).HasMaxLength(200);
        b.Property(x => x.AltText).HasMaxLength(200);
        b.Property(x => x.ImageUrl).HasMaxLength(500);
        b.Property(x => x.LinkUrl).HasMaxLength(500);
        b.Property(x => x.IsVisible).HasDefaultValue(true);
        b.Property(x => x.RowVersion).IsRowVersion();
        b.HasIndex(x => new { x.IsVisible, x.SortOrder });
    }
}

public class TestimonialConfiguration : IEntityTypeConfiguration<Testimonial>
{
    public void Configure(EntityTypeBuilder<Testimonial> b)
    {
        b.Property(x => x.CustomerName).HasMaxLength(200);
        b.Property(x => x.CustomerTitle).HasMaxLength(200);
        b.Property(x => x.CustomerAvatarUrl).HasMaxLength(500);
        b.Property(x => x.Content).HasMaxLength(2000);
        b.Property(x => x.IsVisible).HasDefaultValue(true);
        b.Property(x => x.RowVersion).IsRowVersion();
        b.HasIndex(x => new { x.IsVisible, x.SortOrder });
    }
}

public class FaqConfiguration : IEntityTypeConfiguration<Faq>
{
    public void Configure(EntityTypeBuilder<Faq> b)
    {
        b.Property(x => x.Question).HasMaxLength(500);
        b.Property(x => x.AskerName).HasMaxLength(200);
        b.Property(x => x.AskerEmail).HasMaxLength(200);
        b.Property(x => x.SearchText).HasMaxLength(2000);
        b.Property(x => x.IsVisible).HasDefaultValue(true);
        b.Property(x => x.RowVersion).IsRowVersion();
        b.HasIndex(x => new { x.IsVisible, x.SortOrder });
    }
}

public class AlbumConfiguration : IEntityTypeConfiguration<Album>
{
    public void Configure(EntityTypeBuilder<Album> b)
    {
        b.Property(x => x.Name).HasMaxLength(200);
        b.Property(x => x.Slug).HasMaxLength(200);
        b.HasIndex(x => x.Slug).IsUnique();
        b.Property(x => x.Summary).HasMaxLength(1000);
        b.Property(x => x.ThumbnailUrl).HasMaxLength(500);
        b.Property(x => x.SearchText).HasMaxLength(2000);
        b.Property(x => x.IsVisible).HasDefaultValue(true);
        b.Property(x => x.RowVersion).IsRowVersion();

        b.HasMany(x => x.Images)
            .WithOne(x => x.Album)
            .HasForeignKey(x => x.AlbumId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class AlbumImageConfiguration : IEntityTypeConfiguration<AlbumImage>
{
    public void Configure(EntityTypeBuilder<AlbumImage> b)
    {
        b.Property(x => x.ImageUrl).HasMaxLength(500);
        b.Property(x => x.Caption).HasMaxLength(300);
    }
}

public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> b)
    {
        b.Property(x => x.Name).HasMaxLength(200);
        b.Property(x => x.Address).HasMaxLength(500);
        b.Property(x => x.OldAddress).HasMaxLength(500);
        b.Property(x => x.Phone).HasMaxLength(50);
        b.Property(x => x.ZaloPhone).HasMaxLength(50);
        b.Property(x => x.FacebookUrl).HasMaxLength(500);
        b.Property(x => x.FacebookHandle).HasMaxLength(100);
        b.Property(x => x.MapUrl).HasMaxLength(1000);
        b.Property(x => x.OpeningHours).HasMaxLength(200);
        b.Property(x => x.Latitude).HasPrecision(9, 6);
        b.Property(x => x.Longitude).HasPrecision(9, 6);
        b.Property(x => x.IsVisible).HasDefaultValue(true);
        b.Property(x => x.RowVersion).IsRowVersion();
    }
}

public class SiteSettingConfiguration : IEntityTypeConfiguration<SiteSetting>
{
    public void Configure(EntityTypeBuilder<SiteSetting> b)
    {
        b.HasKey(x => x.Key);
        b.Property(x => x.Key).HasMaxLength(100);
        b.Property(x => x.Description).HasMaxLength(500);
    }
}

public class RedirectConfiguration : IEntityTypeConfiguration<Redirect>
{
    public void Configure(EntityTypeBuilder<Redirect> b)
    {
        b.Property(x => x.FromPath).HasMaxLength(500);
        b.HasIndex(x => x.FromPath).IsUnique();
        b.Property(x => x.ToPath).HasMaxLength(500);
    }
}

public class MediaFileConfiguration : IEntityTypeConfiguration<MediaFile>
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

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
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
