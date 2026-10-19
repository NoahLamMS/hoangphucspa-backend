namespace HoangPhucSpa.Data;

public interface ICurrentUser
{
    Guid? Id { get; }
    string? UserName { get; }
}
