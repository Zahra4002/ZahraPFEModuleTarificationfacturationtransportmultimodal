using Application.Common.Validator;
using Application.Features.AuthFeature.Commands;
using FluentValidation;

namespace Application.Features.AuthFeature.Validators
{
    public class LoginCommandValidator : ValidatorBase<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(v => v.LoginRequest.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(v => v.LoginRequest.Password)
                .NotEmpty().WithMessage("Password is required");
        }
    }
}