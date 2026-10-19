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
    }
}
