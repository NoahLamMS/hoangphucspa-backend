namespace HoangPhucSpa.Data.Entities;

public class Testimonial : AuditableEntity
{
    public int Id { get; set; }
    public required string CustomerName { get; set; }
    public string? CustomerTitle { get; set; }
    public string? CustomerAvatarUrl { get; set; }
    public byte Rating { get; set; } = 5;
    public required string Content { get; set; }
    public int SortOrder { get; set; }
    public bool IsVisible { get; set; } = true;
    public byte[] RowVersion { get; set; } = [];
}
