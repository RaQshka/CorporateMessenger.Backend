﻿using FluentValidation;
using Messenger.Application.Users.Commands.RegisterUser;

namespace Messenger.Application.Users.Commands.LoginUser;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MinimumLength(4);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.CorporateKey).NotEmpty().Length(12);
    }
}