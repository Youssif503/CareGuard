using Microsoft.AspNetCore.Identity;

namespace CareGuard.Domain.Models.Identity;

public class AppUser:IdentityUser
{
    public ElderProfile? Elder { get; set; }
    public CareGiverProfile? CareGiver { get; set; }
    public ICollection<RefreshToken>? RefreshTokens { get; set; }
}