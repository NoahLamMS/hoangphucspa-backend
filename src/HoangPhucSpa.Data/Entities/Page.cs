namespace HoangPhucSpa.Data.Entities;

public class Page : AuditableEntity
{
    public int Id { get; set; }
    public required string Key { get; set; }
    public required string Slug { get; set; }
    public required string Title { get; set; }
    public string? Summary { get; set; }
    public string? SeoTitle { get; set; }
    public string? SeoDescription { get; set; }
    public string? SeoKeywords { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? BannerUrl { get; set; }
    public string? ContentHtml { get; set; }
    public bool IsVisible { get; set; } = true;
    public string? SearchText { get; set; }
}
