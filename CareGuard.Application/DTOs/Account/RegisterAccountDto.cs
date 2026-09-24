using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CareGuard.Domain.Enums;

namespace CareGuard.Application.DTOs.Account
{
    public record RegisterAccountDto(ProfileType UserType,string FullName,string Email,string Password,string ConfirmPassword);
}