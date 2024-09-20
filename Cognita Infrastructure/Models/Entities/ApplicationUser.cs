using Cognita_Shared.Entities;
using Microsoft.AspNetCore.Identity;

namespace Cognita_Infrastructure.Models.Entities;

public class ApplicationUser : IdentityUser
{
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpireTime { get; set; }
    public required User User { get; set; }
}
