using CareGuard.Application.Common;
using CareGuard.Application.Common.Interfaces;
using CareGuard.Application.Common.Interfaces.Authentication;
using CareGuard.Application.DTOs;
using CareGuard.Application.DTOs.Authntication;
using CareGuard.Domain.Enums;
using CareGuard.Domain.Models.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CareGuard.Application.Features.Account.Commands.Login;

public class LoginCommandHandler
    : IRequestHandler<LoginCommand, Result<AuthTokensDto>>
{
    private readonly IAppDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly IAuthenticationService _authService;

    public LoginCommandHandler(
        IAppDbContext context,
        UserManager<AppUser> userManager,
        IAuthenticationService authService)
    {
        _context = context;
        _userManager = userManager;
        _authService = authService;
    }

    public async Task<Result<AuthTokensDto>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Find user by email
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return Result<AuthTokensDto>.Failure(
                ["Invalid email or password."]);
        }

        // 2. Check password
        var passwordValid = await _userManager.CheckPasswordAsync(
            user,
            request.Password);

        if (!passwordValid)
        {
            return Result<AuthTokensDto>.Failure(
                ["Invalid email or password."]);
        }

        // 3. Get the requested profile
        UserProfile? profile = null;

        switch (request.Type)
        {
            case ProfileType.Elder:

                var elder = await _context.Elder
                    .FirstOrDefaultAsync(
                        x => x.UserId == user.Id,
                        cancellationToken);

                if (elder is null)
                {
                    return Result<AuthTokensDto>.Failure(
                        ["Elder profile does not exist."]);
                }

                profile = new UserProfile
                {
                    Id = elder.Id,
                    Name = elder.FullName,
                    Type = ProfileType.Elder
                };

                break;

            case ProfileType.CareGiver:

                var caregiver = await _context.CareGiver
                    .FirstOrDefaultAsync(
                        x => x.UserId == user.Id,
                        cancellationToken);

                if (caregiver is null)
                {
                    return Result<AuthTokensDto>.Failure(
                        ["Caregiver profile does not exist."]);
                }

                profile = new UserProfile
                {
                    Id = caregiver.Id,
                    Name = caregiver.FullName,
                    Type = ProfileType.CareGiver
                };

                break;

            default:

                return Result<AuthTokensDto>.Failure(
                    ["Invalid profile type."]);
        }

        // 4. Create Access Token + Refresh Token
        var tokens = await _authService.CreateAuthTokensAsync(
            user,
            profile);

        return Result<AuthTokensDto>.Success(tokens);
    }
}