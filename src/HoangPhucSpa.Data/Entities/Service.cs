namespace HoangPhucSpa.Data.Entities;

public class Service : AuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public string? Summary { get; set; }
    public string? SeoTitle { get; set; }
    public string? SeoDescription { get; set; }
    public string? SeoKeywords { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? BannerUrl { get; set; }
    public string? ContentHtml { get; set; }
    public bool IsVisible { get; set; } = true;
    public int SortOrder { get; set; }
    public string? SearchText { get; set; }
    public DateTime? DeletedAt { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public List<SubService> SubServices { get; set; } = [];
}
