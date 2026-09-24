using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CareGuard.Domain.Enums;
using FluentValidation;

namespace CareGuard.Application.Features.Account.Commands.Login
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x=>x.Email)
            .NotEmpty()
            .EmailAddress();

            RuleFor(x=>x.Password)
            .NotEmpty();

            RuleFor(x=>x.Type)
            .IsInEnum();
        }
    }
}