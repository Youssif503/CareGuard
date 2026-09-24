using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CareGuard.Application.Common;
using CareGuard.Application.DTOs.Authntication;
using CareGuard.Domain.Enums;
using MediatR;

namespace CareGuard.Application.Features.Account.Commands.Register
{
    public record RegisterCommand(ProfileType UserType,string FullName,string Email,string Password,
    string ConfirmPassword,string PhoneNumber):
    IRequest<Result<AuthTokensDto>>;
}