namespace HoangPhucSpa.Data.Entities;

public class Redirect
{
    public int Id { get; set; }
    public required string FromPath { get; set; }
    public required string ToPath { get; set; }
    public short StatusCode { get; set; } = 301;
    public RedirectSource Source { get; set; } = RedirectSource.Manual;
    public int HitCount { get; set; }
    public DateTime? LastHitAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
