using HoangPhucSpa.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HoangPhucSpa.Data.Configurations;

public sealed class FaqConfiguration : IEntityTypeConfiguration<Faq>
{
    public void Configure(EntityTypeBuilder<Faq> b)
    {
        b.Property(x => x.Question).HasMaxLength(1000);
        b.Property(x => x.AskerName).HasMaxLength(200);
        b.Property(x => x.AskerEmail).HasMaxLength(256);
        b.Property(x => x.SearchText).HasMaxLength(2000);
        b.Property(x => x.IsVisible).HasDefaultValue(true);
        b.Property(x => x.RowVersion).IsConcurrencyToken();
        b.HasIndex(x => new { x.IsVisible, x.SortOrder });
    }
}

