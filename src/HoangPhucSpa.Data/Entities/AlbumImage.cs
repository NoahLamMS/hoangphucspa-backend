namespace HoangPhucSpa.Data.Entities;

public class AlbumImage
{
    public int Id { get; set; }
    public int AlbumId { get; set; }
    public Album Album { get; set; } = null!;
    public required string ImageUrl { get; set; }
    public string? Caption { get; set; }
    public int SortOrder { get; set; }
}
