using FluentValidation;

namespace App.Application.PowerOfAttorney.Commands.CreatePowerOfAttorney;

public class CreatePowerOfAttorneyCommandValidator : AbstractValidator<CreatePowerOfAttorneyCommand>
{
    public CreatePowerOfAttorneyCommandValidator()
    {
        RuleFor(p => p.ClientId).NotEmpty().WithMessage("معرف الموكل مطلوب.");
        RuleFor(p => p.PowerNumber).NotEmpty().WithMessage("رقم التوكيل مطلوب.");
    }
}
