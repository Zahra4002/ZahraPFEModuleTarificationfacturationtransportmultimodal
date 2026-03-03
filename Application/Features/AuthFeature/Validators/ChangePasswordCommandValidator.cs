using Application.Common.Validator;
using Application.Features.AuthFeature.Commands;
using FluentValidation;

namespace Application.Features.AuthFeature.Validators
{
    public class ChangePasswordCommandValidator : ValidatorBase<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {
            RuleFor(v => v.CurrentPassword)
                .NotEmpty().WithMessage("Current password is required");

            RuleFor(v => v.NewPassword)
                .NotEmpty().WithMessage("New password is required")
                .MinimumLength(6).WithMessage("New password must be at least 6 characters")
                .NotEqual(v => v.CurrentPassword).WithMessage("New password must be different from current password");
        }
    }
}