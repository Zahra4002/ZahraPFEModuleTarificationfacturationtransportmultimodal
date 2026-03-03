// Application/Features/UserFeature/Validators/AddUserCommandValidator.cs
using Application.Common.Constants;
using Application.Common.Validator;
using Application.Features.UserFeature.Commands;
using FluentValidation;

namespace Application.Features.UserFeature.Validators
{
    public class AddUserCommandValidator : ValidatorBase<AddUserCommand>
    {
        public AddUserCommandValidator()
        {
            RuleFor(v => v.UserDto.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(v => v.UserDto.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters");

            RuleFor(v => v.UserDto.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(50).WithMessage("First name must not exceed 50 characters");

            RuleFor(v => v.UserDto.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(50).WithMessage("Last name must not exceed 50 characters");

            RuleFor(v => v.UserDto.Role)
                .NotEmpty().WithMessage("Role is required")
                .Must(role => new[] { "Administrateur", "Gestionnaire", "Operateur", "Comptable", "Lecture" }.Contains(role))
                .WithMessage("Invalid role. Must be: Administrateur, Gestionnaire, Operateur, Comptable, or Lecture");
        }
    }
}