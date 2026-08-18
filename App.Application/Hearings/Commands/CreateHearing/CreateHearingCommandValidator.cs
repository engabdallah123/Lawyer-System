using FluentValidation;

namespace App.Application.Hearings.Commands.CreateHearing;

public class CreateHearingCommandValidator : AbstractValidator<CreateHearingCommand>
{
    public CreateHearingCommandValidator()
    {
        RuleFor(h => h.CaseId).NotEmpty().WithMessage("معرف القضية مطلوب.");
        RuleFor(h => h.HearingType).NotEmpty().WithMessage("نوع الجلسة مطلوب.");
    }
}
