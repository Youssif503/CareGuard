using CareGuard.Domain.Models.Identity;

namespace CareGuard.Domain.Models;

public class CareGiverProfile
{
    public string Id { get; set; }

    public string UserId { get; set; } = null!;

    public AppUser User { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Address { get; set; }

    public string? Bio { get; set; }
}