using System.Security.Claims;
using HoangPhucSpa.Data;

namespace HoangPhucSpa.Api.Infrastructure;

public sealed class HttpCurrentUser(IHttpContextAccessor http) : ICurrentUser
{
    public Guid? Id =>
        Guid.TryParse(http.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;

    public string? UserName => http.HttpContext?.User.Identity?.Name;
}
