namespace HoangPhucSpa.Data.Entities;

public class Album : AuditableEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public string? Summary { get; set; }
    public string? ThumbnailUrl { get; set; }
    public int SortOrder { get; set; }
    public bool IsVisible { get; set; } = true;
    public string? SearchText { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public List<AlbumImage> Images { get; set; } = [];
}
