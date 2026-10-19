using Microsoft.AspNetCore.Identity;

namespace HoangPhucSpa.Data.Entities;

public class AppRole : IdentityRole<Guid>
{
    public AppRole() { }
    public AppRole(string roleName) : base(roleName) { }
}
