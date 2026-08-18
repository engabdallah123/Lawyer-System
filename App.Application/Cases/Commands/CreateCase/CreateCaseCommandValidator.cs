using FluentValidation;

namespace App.Application.Cases.Commands.CreateCase;

public class CreateCaseCommandValidator : AbstractValidator<CreateCaseCommand>
{
    public CreateCaseCommandValidator()
    {
        RuleFor(c => c.InternalNumber)
            .NotEmpty().WithMessage("الرقم الداخلي للقضية مطلوب.")
            .MaximumLength(50).WithMessage("الرقم الداخلي لا يجب أن يتجاوز 50 حرفًا.");

        RuleFor(c => c.Title)
            .NotEmpty().WithMessage("عنوان القضية مطلوب.")
            .MaximumLength(500).WithMessage("عنوان القضية لا يجب أن يتجاوز 500 حرف.");

        RuleFor(c => c.CaseTypeId)
            .GreaterThan(0).WithMessage("نوع القضية مطلوب.");

        RuleFor(c => c.CaseStatusId)
            .GreaterThan(0).WithMessage("حالة القضية مطلوبة.");
    }
}
