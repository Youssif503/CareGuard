using CareGuard.Domain.Models.Identity;

namespace CareGuard.Domain.Models;

public class CareGiverProfile
{
    public string Id { get; set; }

    public string UserId { get; set; } = null!;
    public string? PhoneNumber {get;set;}

    public AppUser User { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string? Address { get; set; }

    public string? Bio { get; set; }
}