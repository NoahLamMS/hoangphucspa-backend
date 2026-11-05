using HoangPhucSpa.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HoangPhucSpa.Data.Configurations;

public sealed class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> b)
    {
        b.Property(x => x.Name).HasMaxLength(200);
        b.Property(x => x.Slug).HasMaxLength(200);
        b.HasIndex(x => x.Slug).IsUnique().HasFilter("\"DeletedAt\" IS NULL");
        b.Property(x => x.Summary).HasMaxLength(1000);
        b.Property(x => x.SeoTitle).HasMaxLength(200);
        b.Property(x => x.SeoDescription).HasMaxLength(500);
        b.Property(x => x.SeoKeywords).HasMaxLength(500);
        b.Property(x => x.ThumbnailUrl).HasMaxLength(500);
        b.Property(x => x.BannerUrl).HasMaxLength(500);
        b.Property(x => x.SearchText).HasMaxLength(2000);
        b.Property(x => x.IsVisible).HasDefaultValue(true);
        b.Property(x => x.RowVersion).IsConcurrencyToken();
        b.HasIndex(x => new { x.IsVisible, x.SortOrder }).HasFilter("\"DeletedAt\" IS NULL");
        b.HasQueryFilter(x => x.DeletedAt == null);

        b.HasMany(x => x.SubServices)
            .WithOne(x => x.Service)
            .HasForeignKey(x => x.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

