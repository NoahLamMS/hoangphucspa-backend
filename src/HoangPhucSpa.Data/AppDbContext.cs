using HoangPhucSpa.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HoangPhucSpa.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<AppUser, AppRole, Guid>(options)
{
    public DbSet<Service> Services => Set<Service>();
    public DbSet<SubService> SubServices => Set<SubService>();
    public DbSet<PostCategory> PostCategories => Set<PostCategory>();
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Page> Pages => Set<Page>();
    public DbSet<Slide> Slides => Set<Slide>();
    public DbSet<Testimonial> Testimonials => Set<Testimonial>();
    public DbSet<Faq> Faqs => Set<Faq>();
    public DbSet<Album> Albums => Set<Album>();
    public DbSet<AlbumImage> AlbumImages => Set<AlbumImage>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<SiteSetting> SiteSettings => Set<SiteSetting>();
    public DbSet<Redirect> Redirects => Set<Redirect>();
    public DbSet<MediaFile> MediaFiles => Set<MediaFile>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void ConfigureConventions(ModelConfigurationBuilder b)
    {
        base.ConfigureConventions(b);
        b.Properties<DateTime>().HaveConversion<UtcDateTimeConverter>();
    }

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);
        b.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        b.Entity<AppRole>().HasData(
            new AppRole("Admin")
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                NormalizedName = "ADMIN",
                ConcurrencyStamp = "11111111-1111-1111-1111-111111111111",
            },
            new AppRole("Editor")
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                NormalizedName = "EDITOR",
                ConcurrencyStamp = "22222222-2222-2222-2222-222222222222",
            }
        );
    }
}
