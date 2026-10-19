namespace HoangPhucSpa.Data.Entities;

public class MediaFile
{
    public int Id { get; set; }
    public required string Path { get; set; }
    public required string OriginalFileName { get; set; }
    public required string ContentType { get; set; }
    public long SizeBytes { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public string? Sha256 { get; set; }
    public string? AltText { get; set; }
    public MediaSource Source { get; set; } = MediaSource.Upload;
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedById { get; set; }
}
