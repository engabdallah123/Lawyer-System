using FluentValidation;

namespace App.Application.Consultations.Commands.CreateConsultation;

public class CreateConsultationCommandValidator : AbstractValidator<CreateConsultationCommand>
{
    public CreateConsultationCommandValidator()
    {
        RuleFor(c => c.ClientId).NotEmpty().WithMessage("معرف الموكل مطلوب.");
        RuleFor(c => c.Subject).NotEmpty().WithMessage("موضوع الاستشارة مطلوب.");
    }
}
