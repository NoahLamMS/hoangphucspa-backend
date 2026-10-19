namespace HoangPhucSpa.Data.Entities;

public class Branch : AuditableEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Address { get; set; }
    public string? OldAddress { get; set; }
    public required string Phone { get; set; }
    public string? ZaloPhone { get; set; }
    public string? FacebookUrl { get; set; }
    public string? FacebookHandle { get; set; }
    public string? MapUrl { get; set; }
    public string? MapEmbedHtml { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? OpeningHours { get; set; }
    public int SortOrder { get; set; }
    public bool IsVisible { get; set; } = true;
    public byte[] RowVersion { get; set; } = [];
}
