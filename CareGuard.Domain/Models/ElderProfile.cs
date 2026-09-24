using CareGuard.Domain.Models.Identity;

namespace CareGuard.Domain.Models;

public class ElderProfile
{
    public string Id { get; set; }

    public string UserId { get; set; } = null!;
    public string? PhoneNumber {get;set;}

    public AppUser? User { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public DateOnly? DateOfBirth { get; set; }

    public string? Address { get; set; }

    public string? EmergencyContact { get; set; }
    
    public ICollection<Medication> Medications { get; set; }
        = new List<Medication>();
}