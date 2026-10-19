namespace HoangPhucSpa.Data.Entities;

public interface ISoftDeletable
{
    DateTime? DeletedAt { get; set; }
}
