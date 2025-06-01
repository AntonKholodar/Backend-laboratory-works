using ChatApp.Domain.ValueObjects;
using FluentValidation;

namespace ChatApp.Application.Features.Users.Commands.RegisterUser;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .Length(2, 100).WithMessage("Name must be between 2 and 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .Must(BeValidEmail).WithMessage("Email format is invalid");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters");

        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("Valid gender is required");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required")
            .Must(BeValidAge).WithMessage("Age must be between 13 and 120 years");
    }

    private bool BeValidEmail(string email)
    {
        try
        {
            new Email(email); // This will throw if invalid
            return true;
        }
        catch
        {
            return false;
        }
    }

    private bool BeValidAge(DateTime dateOfBirth)
    {
        var age = DateTime.Today.Year - dateOfBirth.Year;
        if (dateOfBirth.Date > DateTime.Today.AddYears(-age)) age--;
        return age >= 13 && age <= 120;
    }
} 