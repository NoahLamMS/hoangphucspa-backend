namespace HoangPhucSpa.Data.Entities;

public class Testimonial : AuditableEntity
{
    public int Id { get; set; }
    public required string AuthorName { get; set; }
    public string? AuthorTitle { get; set; }
    public string? AvatarUrl { get; set; }
    public byte Rating { get; set; } = 5;
    public required string ContentHtml { get; set; }
    public int SortOrder { get; set; }
    public bool IsVisible { get; set; } = true;
}
