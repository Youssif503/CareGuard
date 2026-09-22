using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CareGuard.Application.Common;
using CareGuard.Application.Common.Interfaces.Authentication;
using CareGuard.Application.DTOs;
using CareGuard.Application.DTOs.Authntication;
using CareGuard.Domain.Enums;
using CareGuard.Domain.Models;
using CareGuard.Domain.Models.Identity;
using CareGuard.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
namespace CareGuard.Infrastructure.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly UserManager<AppUser> _userManager;

    public AuthenticationService(AppDbContext context, IConfiguration configuration, UserManager<AppUser> userManager)
    {
        _configuration = configuration;
        _userManager = userManager;
        _context = context;
    }
    public async Task<string> CreateAccessTokenAsync(AppUser user, UserProfile profile)
    {
        var userRoles = await _userManager.GetRolesAsync(user);
        var roleClaims = userRoles.Select(role => new Claim("role", role));
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, profile.Name),
            new("profile_id", profile.Id.ToString()),
            new("profile_type", profile.Type.ToString())
        }
        .Union(roleClaims);



        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"]!));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        int.TryParse(
            _configuration["JwtConfiguration:AccessDurationInHour"],
                out int duration);

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtConfiguration:Issuer"],
            audience: _configuration["JwtConfiguration:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(duration),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }

    public async Task<AuthTokensDto> CreateAuthTokensAsync(AppUser user, UserProfile userProfile)
    {
        int.TryParse(
            _configuration["JwtConfiguration:AccessDurationInHour"],
                out int AccessTokenDuration);
        int.TryParse(
            _configuration["JwtConfiguration:RefreshTokenDurationInDays"],
                out int RefreshTokenDuration);
        var accessTokenExpiresAt = DateTime.UtcNow.AddHours(AccessTokenDuration);
        var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(RefreshTokenDuration);
        var refreshToken = GenerateRefreshToken();
        var refreshTokenHash = HashRefreshToken(refreshToken);

        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            ProfileId = userProfile.Id,
            ProfileType = userProfile.Type,
            TokenHash = refreshTokenHash,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(
        RefreshTokenDuration)
        };
        await _context.RefreshToken.AddAsync(refreshTokenEntity);
        await _context.SaveChangesAsync();

        return new AuthTokensDto
        {
            Name = userProfile.Name,
            ProfileType = userProfile.Type.ToString(),
            Email = user.Email!,
            AccessToken = await CreateAccessTokenAsync(user, userProfile),
            RefreshToken = refreshToken,
            AccessTokenExpiresAt = accessTokenExpiresAt,
            RefreshTokenExpiresAt = refreshTokenExpiresAt
        };

    }

    public async Task<AuthTokensDto?> RefreshTokensAsync(
    string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return null;

        var tokenHash = HashRefreshToken(refreshToken);

        var storedToken = await _context.RefreshToken
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash);

        if (storedToken is null || !storedToken.IsActive)
            return null;

        var userProfile = await GetUserProfileAsync(
            storedToken.ProfileId,
            storedToken.ProfileType);

        if (userProfile is null)
            return null;

        storedToken.RevokedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await CreateAuthTokensAsync(
            storedToken.User,
            userProfile);
    }
    public async Task<bool> RevokeRefreshTokenAsync(
        string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return false;

        var tokenHash = HashRefreshToken(refreshToken);

        var storedToken = await _context.RefreshToken
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash);

        if (storedToken is null || !storedToken.IsActive)
            return false;

        storedToken.RevokedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RevokeAllRefreshTokensAsync(
        string userId)
    {
        var tokens = await _context.RefreshToken
            .Where(x =>
                x.UserId == userId &&
                x.RevokedAt == null &&
                x.ExpiresAt > DateTime.UtcNow)
            .ToListAsync();

        if (tokens.Count == 0)
            return false;

        foreach (var token in tokens)
        {
            token.RevokedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return true;
    }
    private string GenerateRefreshToken()
    {
        var bytes = new byte[64];

        RandomNumberGenerator.Fill(bytes);

        return Convert.ToBase64String(bytes);
    }
    private string HashRefreshToken(string RefreshToken)
    {
        var hashing = SHA256.HashData(Encoding.UTF8.GetBytes(RefreshToken));
        return Convert.ToBase64String(hashing);
    }

    private async Task<UserProfile?> GetUserProfileAsync(string profileId, ProfileType profileType)
    {
        if (profileType == ProfileType.CareGiver)
        {
            var caregiver = await _context.CareGiver
                .FirstOrDefaultAsync(x => x.Id == profileId);

            if (caregiver is null)
                return null;

            return new UserProfile
            {
                Id = caregiver.Id,
                Name = string.Concat(caregiver.FirstName, " ", caregiver.LastName),
                Type = ProfileType.CareGiver
            };
        }

        if (profileType == ProfileType.Elder)
        {
            var elder = await _context.Elder
                .FirstOrDefaultAsync(x => x.Id == profileId);

            if (elder is null)
                return null;

            return new UserProfile
            {
                Id = elder.Id,
                Name = string.Concat(elder.FirstName, " ", elder.LastName),
                Type = ProfileType.Elder
            };
        }

        return null;
    }
}