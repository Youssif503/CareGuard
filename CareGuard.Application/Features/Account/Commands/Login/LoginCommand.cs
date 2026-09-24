using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CareGuard.Application.Common;
using CareGuard.Application.DTOs.Authntication;
using CareGuard.Domain.Enums;
using MediatR;

namespace CareGuard.Application.Features.Account.Commands.Login
{
    public record LoginCommand(string Email,string Password,ProfileType Type) :IRequest<Result<AuthTokensDto>>;
}