namespace HoangPhucSpa.Data.Entities;

public class Post : AuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public PostKind Kind { get; set; } = PostKind.News;
    public int? LegacyId { get; set; }
    public int PublicId { get; set; }
    public int? CategoryId { get; set; }
    public PostCategory? Category { get; set; }
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public string? Summary { get; set; }
    public string? SeoTitle { get; set; }
    public string? SeoDescription { get; set; }
    public string? SeoKeywords { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? BannerUrl { get; set; }
    public string? ContentHtml { get; set; }
    public DateTime PublishedAt { get; set; }
    public int ViewCount { get; set; }
    public bool IsHot { get; set; }
    public bool IsVisible { get; set; } = true;
    public int SortOrder { get; set; }
    public string? SearchText { get; set; }
    public DateTime? DeletedAt { get; set; }
    public byte[] RowVersion { get; set; } = [];
}
