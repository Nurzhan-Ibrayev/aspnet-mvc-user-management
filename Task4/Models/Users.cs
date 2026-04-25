using Microsoft.AspNetCore.Identity;

namespace Task4.Models;

public class Users : IdentityUser
{
    public DateTime? LastLoginTime { get; set; }
    public string Status { get; set; } = "Unverified";


}
