namespace HoangPhucSpa.Data.Entities;

public class AuditLog
{
    public long Id { get; set; }
    public DateTime OccurredAt { get; set; }
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public required string Action { get; set; }
    public required string EntityType { get; set; }
    public required string EntityId { get; set; }
    public string? Summary { get; set; }
    public string? IpAddress { get; set; }
    public string? ChangesJson { get; set; }
}
