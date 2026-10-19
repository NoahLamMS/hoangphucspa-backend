namespace HoangPhucSpa.Data.Entities;

public class Slide : AuditableEntity
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? AltText { get; set; }
    public required string ImageUrl { get; set; }
    public string? LinkUrl { get; set; }
    public int SortOrder { get; set; }
    public bool IsVisible { get; set; } = true;
    public byte[] RowVersion { get; set; } = [];
}
