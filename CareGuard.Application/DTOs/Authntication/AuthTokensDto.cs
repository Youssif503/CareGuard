namespace CareGuard.Application.DTOs.Authntication;
public class AuthTokensDto
{
    public string Name { get; set; } = string.Empty;
    public string ProfileType { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime AccessTokenExpiresAt { get; set; }
    public DateTime RefreshTokenExpiresAt { get; set; }
}