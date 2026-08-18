using FluentValidation;

namespace App.Application.Cases.Commands.UpdateCase;

public class UpdateCaseCommandValidator : AbstractValidator<UpdateCaseCommand>
{
    public UpdateCaseCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty().WithMessage("معرف القضية مطلوب.");
        RuleFor(c => c.InternalNumber).NotEmpty().WithMessage("الرقم الداخلي للقضية مطلوب.");
        RuleFor(c => c.Title).NotEmpty().WithMessage("عنوان القضية مطلوب.");
    }
}
