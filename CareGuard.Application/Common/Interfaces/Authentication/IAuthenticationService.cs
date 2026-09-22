using CareGuard.Application.DTOs;
using CareGuard.Application.DTOs.Authntication;
using CareGuard.Domain.Models.Identity;

namespace CareGuard.Application.Common.Interfaces.Authentication;

public interface IAuthenticationService
{
    Task<string> CreateAccessTokenAsync(AppUser user,UserProfile profile);

    Task<AuthTokensDto> CreateAuthTokensAsync(AppUser user,UserProfile profile);

    Task<AuthTokensDto?> RefreshTokensAsync(string refreshToken);

    Task<bool> RevokeRefreshTokenAsync(string refreshToken);

    Task<bool> RevokeAllRefreshTokensAsync(string userId);
}