using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace CareGuard.Application.Features.Account.Commands.Register
{
    public class RegisterCommandVlidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandVlidator()
        {
            RuleFor(x=>x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Email Required Yastaaa");

            RuleFor(x => x.UserType)
            .IsInEnum();

            RuleFor(x=>x.FullName)
            .NotEmpty()
            .WithMessage("Need Full Name Ya3m");

            RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8);

            RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .Equal(x => x.Password)
            .WithMessage("Passwords do not match Ya3m");
        }
    }
}