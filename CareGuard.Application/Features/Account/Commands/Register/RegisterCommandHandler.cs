using CareGuard.Application.Common;
using CareGuard.Application.Common.Interfaces;
using CareGuard.Application.Common.Interfaces.Authentication;
using CareGuard.Application.DTOs;
using CareGuard.Application.DTOs.Authntication;
using CareGuard.Domain.Enums;
using CareGuard.Domain.Models;
using CareGuard.Domain.Models.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CareGuard.Application.Features.Account.Commands.Register;

public class RegisterCommandHandler
    : IRequestHandler<RegisterCommand, Result<AuthTokensDto>>
{
    private readonly IAppDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly IAuthenticationService _authService;

    public RegisterCommandHandler(
        IAppDbContext context,
        UserManager<AppUser> userManager,
        IAuthenticationService authService)
    {
        _context = context;
        _userManager = userManager;
        _authService = authService;
    }

    public async Task<Result<AuthTokensDto>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                cancellationToken);

        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null)
            {
                user = new AppUser
                {
                    UserName = request.Email,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber
                };

                var createResult = await _userManager.CreateAsync(
                    user,
                    request.Password);

                if (!createResult.Succeeded)
                {
                    var errors = createResult.Errors.Select(x => x.Description).ToArray();

                    await transaction.RollbackAsync(cancellationToken);

                    return Result<AuthTokensDto>.Failure(errors);
                }
            }

            UserProfile profile;

            switch (request.UserType)
            {
                case ProfileType.Elder:

                    var elderExists = await _context.Elder
                        .AnyAsync(
                            x => x.UserId == user.Id,
                            cancellationToken);

                    if (elderExists)
                    {
                        await transaction.RollbackAsync(cancellationToken);

                        return Result<AuthTokensDto>.Failure(
                            ["Elder profile already exists."]);
                    }

                    var elder = new ElderProfile
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = user.Id,
                        FullName = request.FullName,
                        PhoneNumber = request.PhoneNumber
                    };

                    await _context.Elder.AddAsync(
                        elder,
                        cancellationToken);

                    profile = new UserProfile
                    {
                        Id = elder.Id,
                        Name = elder.FullName,
                        Type = ProfileType.Elder
                    };

                    break;

                case ProfileType.CareGiver:

                    var caregiverExists = await _context.CareGiver
                        .AnyAsync(
                            x => x.UserId == user.Id,
                            cancellationToken);

                    if (caregiverExists)
                    {
                        await transaction.RollbackAsync(cancellationToken);

                        return Result<AuthTokensDto>.Failure(
                            ["Caregiver profile already exists."]);
                    }

                    var caregiver = new CareGiverProfile
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = user.Id,
                        FullName = request.FullName,
                        PhoneNumber = request.PhoneNumber
                    };

                    await _context.CareGiver.AddAsync(
                        caregiver,
                        cancellationToken);

                    profile = new UserProfile
                    {
                        Id = caregiver.Id,
                        Name = caregiver.FullName,
                        Type = ProfileType.CareGiver
                    };

                    break;

                default:

                    await transaction.RollbackAsync(cancellationToken);

                    return Result<AuthTokensDto>.Failure(
                        ["Invalid profile type."]);
            }

            var roleResult = await _userManager.AddToRoleAsync(
                user,
                request.UserType.ToString());

            if (!roleResult.Succeeded)
            {
                var errors = roleResult.Errors.Select(x => x.Description).ToArray();

                await transaction.RollbackAsync(cancellationToken);

                return Result<AuthTokensDto>.Failure(errors);
            }

            await _context.SaveChangesAsync(cancellationToken);

            var tokens = await _authService.CreateAuthTokensAsync(
                user,
                profile);

            await transaction.CommitAsync(cancellationToken);

            return Result<AuthTokensDto>.Success(tokens);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}