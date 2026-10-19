namespace HoangPhucSpa.Data.Entities;

public class SiteSetting
{
    public required string Key { get; set; }
    public required string Value { get; set; }
    public string? Description { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid? UpdatedById { get; set; }
}
