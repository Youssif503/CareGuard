using CareGuard.Domain.Enums;

namespace CareGuard.Application.DTOs
{
    public record UserProfile
    {
    public string Id { get; set; }
    public string Name { get; set; } = null!;
    public ProfileType Type { get; set; }
    }
}