using FluentValidation;

namespace App.Application.Cases.Commands.CreateCase;

public class CreateCaseCommandValidator : AbstractValidator<CreateCaseCommand>
{
    public CreateCaseCommandValidator()
    {
        RuleFor(c => c.ClientId)
            .NotEmpty().WithMessage("يجب اختيار الموكل صاحب ملف القضية.");

        RuleFor(c => c.InternalNumber)
            .NotEmpty().WithMessage("الرقم الداخلي للقضية مطلوب.")
            .MaximumLength(50).WithMessage("الرقم الداخلي لا يجب أن يتجاوز 50 حرفًا.");

        RuleFor(c => c.Title)
            .NotEmpty().WithMessage("موضوع وعنوان القضية مطلوب.")
            .MaximumLength(500).WithMessage("عنوان القضية لا يجب أن يتجاوز 500 حرف.");

        RuleFor(c => c.CaseTypeId)
            .GreaterThan(0).WithMessage("نوع القضية مطلوب.");

        RuleFor(c => c.CaseStatusId)
            .GreaterThan(0).WithMessage("حالة القضية مطلوبة.");

        RuleFor(c => c.OpenDate)
            .NotEmpty().WithMessage("تاريخ فتح الملف مطلوب.");

        RuleFor(c => c.CourtNumber)
            .MaximumLength(100).WithMessage("رقم القضية في المحكمة لا يتجاوز 100 حرف.")
            .When(c => !string.IsNullOrEmpty(c.CourtNumber));

        RuleFor(c => c.ClaimAmount)
            .GreaterThanOrEqualTo(0).WithMessage("مبلغ المطالبة يجب أن يكون قيمة موجبة.")
            .When(c => c.ClaimAmount.HasValue);

        RuleForEach(c => c.AdditionalParties).ChildRules(party =>
        {
            party.RuleFor(p => p.PartyName)
                .NotEmpty().WithMessage("اسم الطرف / الخصم مطلوب.")
                .MaximumLength(200).WithMessage("اسم الطرف لا يتجاوز 200 حرف.")
                .When(p => !p.ClientId.HasValue);

            party.RuleFor(p => p.Email)
                .EmailAddress().WithMessage("صيغة البريد الإلكتروني للطرف غير صحيحة.")
                .When(p => !string.IsNullOrWhiteSpace(p.Email));
        });
    }
}
